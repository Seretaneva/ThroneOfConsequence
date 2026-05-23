using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaEvaluator : MonoBehaviour, IResponseEvaluator
{
    [SerializeField] private string modelName = "llama3.2:3b";
    [SerializeField] private string apiUrl = "http://127.0.0.1:11434/api/chat";
    [SerializeField] private int timeoutSeconds = 8;

    public IEnumerator EvaluateResponse(
        string eventTitle,
        string eventDescription,
        string playerResponse,
        Action<StatEvaluationResult> onSuccess,
        Action<string> onError)
    {
        string systemPrompt =
            "Esti evaluator pentru un joc medieval de strategie. " +
            "Raspunde doar cu JSON valid. Fara explicatii extra.";

        string userPrompt =
$@"Evalueaza decizia jucatorului.

Eveniment: {eventTitle}
Descriere: {eventDescription}
Raspuns jucator: {playerResponse}

Returneaza DOAR JSON valid.

Reguli:
- goldEffect pozitiv doar daca jucatorul castiga bani direct.
- goldEffect negativ daca trimite oameni, armata, agenti sau construieste ceva.
- Nu da +10 decat pentru castiguri uriase.
- Pentru actiuni normale foloseste valori intre -4 si +4.
- respectEffect pozitiv daca oamenii, negustorii sau taranii apreciaza decizia.
- intelligenceEffect pozitiv daca decizia este discreta, strategica sau bine calculata.
- reason trebuie sa fie in romana, maximum 8 cuvinte.
- Nu explica. Nu analiza in text. Doar JSON.

Format:
{{
  ""goldEffect"": 0,
  ""respectEffect"": 0,
  ""intelligenceEffect"": 0,
  ""reason"": """"
}}";

        OllamaChatRequest requestData = new OllamaChatRequest
        {
            model = modelName,
            stream = false,
            format = "json",
            messages = new OllamaMessage[]
            {
                new OllamaMessage { role = "system", content = systemPrompt },
                new OllamaMessage { role = "user", content = userPrompt }
            },
            options = new OllamaOptions
            {
                temperature = 0.2f,
                num_predict = 80
            }
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = timeoutSeconds;

            Debug.Log("Trimit catre Ollama...");
            Debug.Log("Request JSON: " + json);

            yield return request.SendWebRequest();

            Debug.Log("Request result: " + request.result);
            Debug.Log("Request error: " + request.error);
            Debug.Log("Raspuns brut Ollama: " + request.downloadHandler.text);

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke("Eroare la conectarea cu Ollama: " + request.error);
                yield break;
            }

            string responseText = request.downloadHandler.text;

            OllamaChatResponse response;

            try
            {
                response = JsonUtility.FromJson<OllamaChatResponse>(responseText);
            }
            catch
            {
                onError?.Invoke("Raspuns invalid de la Ollama: " + responseText);
                yield break;
            }

            if (response == null || response.message == null || string.IsNullOrWhiteSpace(response.message.content))
            {
                onError?.Invoke("Ollama nu a returnat continut.");
                yield break;
            }

            string content = response.message.content.Trim();

            Debug.Log("JSON generat de model: " + content);

            StatEvaluationResult result;

            try
            {
                result = JsonUtility.FromJson<StatEvaluationResult>(content);
            }
            catch
            {
                onError?.Invoke("JSON-ul din raspunsul Ollama nu a putut fi citit: " + content);
                yield break;
            }

            if (result == null)
            {
                onError?.Invoke("Rezultatul evaluarii este null.");
                yield break;
            }

            result.goldEffect = Mathf.Clamp(result.goldEffect, -10, 10);
            result.respectEffect = Mathf.Clamp(result.respectEffect, -10, 10);
            result.intelligenceEffect = Mathf.Clamp(result.intelligenceEffect, -10, 10);

            if (string.IsNullOrWhiteSpace(result.reason))
                result.reason = "Curtea asteapta efectele deciziei.";

            Debug.Log("RESULT PARSED:");
            Debug.Log("Gold: " + result.goldEffect);
            Debug.Log("Respect: " + result.respectEffect);
            Debug.Log("Intelligence: " + result.intelligenceEffect);
            Debug.Log("Reason: " + result.reason);

            onSuccess?.Invoke(result);
        }
    }
}

[Serializable]
public class OllamaChatRequest
{
    public string model;
    public bool stream;
    public string format;
    public OllamaMessage[] messages;
    public OllamaOptions options;
}

[Serializable]
public class OllamaMessage
{
    public string role;
    public string content;
}

[Serializable]
public class OllamaOptions
{
    public float temperature;
    public int num_predict;
}

[Serializable]
public class OllamaChatResponse
{
    public OllamaMessage message;
}