using System.Reflection;
using webapi.Model;
using webapi.Model.Modifiers;

namespace webapi.Utility
{
    public class AvailableModifierResolver
    {
        //TODO: can probably cache this
        public static List<Modifier> GetPotentialModifiers(CustomObject customObject, CustomField field)
        {
            List<Modifier> modifiers = new List<Modifier>();
            List<Type> PotentialModifiers = Assembly.GetExecutingAssembly().GetTypes().Where(t => t != typeof(Modifier) &&
                                                      typeof(Modifier).IsAssignableFrom(t)).ToList();

            foreach(Type modifier in PotentialModifiers)
            {
                Modifier castModifier = (Modifier)Activator.CreateInstance(modifier);
                if (castModifier.CanTargetProperty(customObject, field))
                    modifiers.Add(castModifier);
            }

            return modifiers;
        }
    }
}
