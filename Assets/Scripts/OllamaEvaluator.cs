using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaEvaluator : MonoBehaviour, IResponseEvaluator
{
    [SerializeField] private string modelName = "gemma3:12b";
    [SerializeField] private string apiUrl = "http://localhost:11434/api/chat";

    public IEnumerator EvaluateResponse(
        string eventTitle,
        string eventDescription,
        string playerResponse,
        Action<StatEvaluationResult> onSuccess,
        Action<string> onError)
    {
        string systemPrompt =
            "Esti evaluatorul unui joc medieval de strategie. " +
            "Analizeaza raspunsul jucatorului si returneaza STRICT JSON valid " +
            "Valorile goldEffect, respectEffect si intelligenceEffect trebuie sa fie intre -10 si 10. " +
            "Judeca raspunsul dupa compasiune, dreptate, pragmatism, autoritate si intelepciune. " +
            "Nu adauga text in afara JSON-ului."+
            "Campul \"reason\" trebuie sa fie EXACT o singura propozitie, maximum 12 cuvinte.";

        string userPrompt =
            $"Event title: {eventTitle}\n" +
            $"Event description: {eventDescription}\n" +
            $"Player response: {playerResponse}\n\n" +

            "Evalueaza raspunsul jucatorului.\n" +
            "Reguli:\n" +
            "- Valorile goldEffect, respectEffect si intelligenceEffect trebuie sa fie intre -10 si 10.\n" +
            "- Daca raspunsul este generos sau ajuta oamenii, creste respectEffect.\n" +
            "- Daca raspunsul este lacom sau egoist, creste goldEffect dar scade respectEffect.\n" +
            "- Daca raspunsul este inteligent sau strategic, creste intelligenceEffect.\n" +
            "- Daca raspunsul este crud sau nedrept, scade respectEffect.\n" +
            "- Campul reason trebuie sa fie O SINGURA PROPOZITIE scurta (max 12 cuvinte).\n\n" +

            "Returneaza STRICT JSON valid in acest format:\n" +
            "{\n" +
            "  \"goldEffect\": 0,\n" +
            "  \"respectEffect\": 0,\n" +
            "  \"intelligenceEffect\": 0,\n" +
            "  \"reason\": \"o singura propozitie scurta\"\n" +
            "}";
        
        OllamaChatRequest requestData = new OllamaChatRequest
        {
            model = modelName,
            stream = false,
            format = new JsonSchemaFormat
            {
                type = "object",
                properties = new SchemaProperties
                {
                    goldEffect = new SchemaType { type = "integer" },
                    respectEffect = new SchemaType { type = "integer" },
                    intelligenceEffect = new SchemaType { type = "integer" },
                    reason = new SchemaType { type = "string" }
                },
                required = new string[] { "goldEffect", "respectEffect", "intelligenceEffect", "reason" }
            },
            messages = new OllamaMessage[]
            {
                new OllamaMessage { role = "system", content = systemPrompt },
                new OllamaMessage { role = "user", content = userPrompt }
            },
            options = new OllamaOptions
            {
                temperature = 0
            }
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

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
            onError?.Invoke("Raspuns invalid de la Ollama.");
            yield break;
        }

        if (response == null || response.message == null || string.IsNullOrWhiteSpace(response.message.content))
        {
            onError?.Invoke("Ollama nu a returnat continut.");
            yield break;
        }

        StatEvaluationResult result;
        try
        {
            result = JsonUtility.FromJson<StatEvaluationResult>(response.message.content);
        }
        catch
        {
            onError?.Invoke("JSON-ul din raspunsul Ollama nu a putut fi citit.");
            yield break;
        }

        result.goldEffect = Mathf.Clamp(result.goldEffect, -10, 10);
        result.respectEffect = Mathf.Clamp(result.respectEffect, -10, 10);
        result.intelligenceEffect = Mathf.Clamp(result.intelligenceEffect, -10, 10);

        onSuccess?.Invoke(result);
    }
}

[Serializable]
public class OllamaChatRequest
{
    public string model;
    public bool stream;
    public JsonSchemaFormat format;
    public OllamaMessage[] messages;
    public OllamaOptions options;
}

[Serializable]
public class JsonSchemaFormat
{
    public string type;
    public SchemaProperties properties;
    public string[] required;
}

[Serializable]
public class SchemaProperties
{
    public SchemaType goldEffect;
    public SchemaType respectEffect;
    public SchemaType intelligenceEffect;
    public SchemaType reason;
}

[Serializable]
public class SchemaType
{
    public string type;
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
    public int temperature;
}

[Serializable]
public class OllamaChatResponse
{
    public OllamaMessage message;
}