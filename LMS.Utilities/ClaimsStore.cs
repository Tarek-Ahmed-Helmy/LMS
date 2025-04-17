using System.Security.Claims;

namespace LMS.Utilities;

public static class ClaimsStore
{
    public static Dictionary<string, List<string>> ControllerClaims = new Dictionary<string, List<string>>()
    {
        //{ "Controller", new List<string> { "Action", "Action", "Action", "Action", "Action" } }
    };

    //Generates claims dynamically in the format: Controller.Action
    public static List<Claim> GetAllClaims()
    {
        return ControllerClaims.SelectMany(kv =>
            kv.Value.Select(action => new Claim($"{kv.Key}.{action}", $"{kv.Key}.{action}"))).ToList();
    }

    public static List<string> GetAllPolicies()
    {
        return GetAllClaims().Select(c => c.Type).ToList();
    }
}
