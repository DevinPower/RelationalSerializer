using Newtonsoft.Json.Linq;
using webapi.DAL;
using webapi.Model.Modifiers;
using webapi.Model;
using Newtonsoft.Json;
using webapi.Model;

namespace webapi.Utility
{
    public class ModifierCaster
    {
        public static List<Modifier> CastIntoModifiers(string modifierDynamic)
        {
            List<Modifier> modifiers = new List<Modifier>();
            List<JObject> modifierObjects = JArray.Parse(modifierDynamic).Cast<JObject>().ToList();
            foreach (JObject modifierObject in modifierObjects)
            {
                string modifierType = modifierObject["Name"].Value<string>();
                JObject underlyingObject = modifierObject["UnderlyingObject"].Value<JObject>();
                Dictionary<string, object> propertyDict = new Dictionary<string, object>();

                if (underlyingObject != null)
                {
                    List<JObject> properties = JArray.Parse(underlyingObject["Properties"].ToString()).Cast<JObject>().ToList();

                    foreach (JObject property in properties)
                    {
                        propertyDict.Add(property["Name"].Value<string>(), property["Value"].Value<object>());
                    }
                }

                Modifier modifier = (Modifier)Activator.CreateInstance(Type.GetType("webapi.Model.Modifiers." + modifierType));
                modifier.LoadUnderlyingObject(false);
                modifier.BuildModifierFromUnderlyingObject(propertyDict);

                modifiers.Add(modifier);
            }

            return modifiers;
        }
    }
}
