using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LIBBRARY_MANAGER.UI.Modules.BookCatalog.Helpers
{
   public static class GetPathHelpers
    {

        /// <summary>
        /// Permet de obtenir une source d'image à partir d'une
        /// adresse string
        /// </summary>
        /// <param name="relativePath">
        /// Route a partir de la racine du projet
        /// </param>
        /// <returns><![CDATA[]]>
        /// </returns>
        public static ImageSource GetPathImagesource(string relativePath)
        {
            // Supprimer le slash avant de combiner
            if (relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
                relativePath = relativePath.Substring(1);

            string baseDir = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
            string absolutePath = Path.Combine(projectRoot, relativePath);

            return new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
        }
        public static  Uri GetPathUri(string relativePath)
        {
            // Supprimer le slash avant de combiner
            if (relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
                relativePath = relativePath.Substring(1);

            string baseDir = AppContext.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
            string absolutePath = Path.Combine(projectRoot, relativePath);

            return new Uri(absolutePath, UriKind.Absolute);
        }

    }
}
