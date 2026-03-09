using System.Collections;

public interface IResponseEvaluator
{
    IEnumerator EvaluateResponse(
        string eventTitle,
        string eventDescription,
        string playerResponse,
        System.Action<StatEvaluationResult> onSuccess,
        System.Action<string> onError
    );
}