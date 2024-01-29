

namespace MedicamentStore.Services
{
    /// <summary>
    /// Helper functions for <see cref="IconType"/>
    /// </summary>
    public static class IconTypeExtensions
    {
        /// <summary>
        /// Converts <see cref="IconType"/> to a FontAwesome string
        /// </summary>
        /// <param name="type">The type to convert</param>
        /// <returns></returns>
        public static string ToFontAwesome(this IconType type)
        {
            // Return a FontAwesome string based on the icon type
            switch (type)
            {
                case IconType.Succes:
                    return "\uf058";

                case IconType.Info:
                    return "\uf05a";

                case IconType.Warning:
                    return "\uf071";

                case IconType.Error:
                    return "\uf06a";
                case IconType.InfoStock:
                    return "\uf468";
                case IconType.InfoPrix:
                    return "\ue529";
                case IconType.InfoExport:
                    return "\uf570";

                // If none found, return null
                default:
                    return null;
            }
        }
    }
}
