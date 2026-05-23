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
            "Esti un judecator narativ pentru un joc medieval de strategie. " +
            "Evaluezi decizia jucatorului strict dupa contextul evenimentului, nu inventezi fapte noi. " +
            "Raspunzi doar cu JSON valid, fara markdown si fara text in afara JSON-ului.";

        string userPrompt =
$@"Evalueaza decizia jucatorului ca un consilier regal lucid si corect.

Eveniment: {eventTitle}
Descriere: {eventDescription}
Raspuns jucator: {playerResponse}

Sarcina:
1. Intelege problema din eveniment.
2. Intelege ce actiune concreta propune jucatorul.
3. Decide efectele probabile asupra aurului, respectului si inteligentei.
4. Scrie un motiv clar pentru jucator, in romana, ca sa inteleaga de ce a primit acele efecte.

Reguli de evaluare:
- Foloseste numai informatiile din eveniment si raspunsul jucatorului.
- Daca raspunsul este vag sau nu rezolva problema, efectele trebuie sa fie mici sau negative.
- Pentru decizii normale foloseste valori intre -3 si +3.
- Foloseste valori intre -4 si -6 sau +4 si +6 doar pentru decizii foarte bune, foarte rele sau costisitoare.
- Nu folosi niciodata valori peste 6 sau sub -6.
- goldEffect creste doar daca decizia aduce bani, taxe, comert, prada sau economie clara.
- goldEffect scade daca decizia consuma resurse: ajutor, soldati, constructii, plati, provizii sau reparatii.
- respectEffect creste daca decizia pare dreapta, miloasa, protectoare sau populara.
- respectEffect scade daca decizia pare cruda, nedreapta, lacoma, ignoranta sau lasa oamenii fara ajutor.
- intelligenceEffect creste daca decizia este strategica, prudenta, investigheaza, negociaza sau previne riscuri.
- intelligenceEffect scade daca decizia este impulsiva, confuza, risipa fara plan sau ignora consecinte evidente.
- Reason trebuie sa fie o singura propozitie coerenta, 12-22 cuvinte, adresata jucatorului.
- Reason trebuie sa explice efectul principal al deciziei, nu sa repete doar scorurile.
- Pastreaza ton medieval simplu si clar, fara poezie exagerata.

Format:
{{
  ""goldEffect"": 0,
  ""respectEffect"": 0,
  ""intelligenceEffect"": 0,
  ""reason"": """"
}}

Returneaza DOAR JSON valid.";

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
                temperature = 0.15f,
                num_predict = 160
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

            result.goldEffect = Mathf.Clamp(result.goldEffect, -6, 6);
            result.respectEffect = Mathf.Clamp(result.respectEffect, -6, 6);
            result.intelligenceEffect = Mathf.Clamp(result.intelligenceEffect, -6, 6);

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
