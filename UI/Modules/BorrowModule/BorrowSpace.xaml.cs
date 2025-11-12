using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LIBBRARY_MANAGER.UI.Modules.BookCatalog.Helpers;

namespace LIBBRARY_MANAGER.UI.Modules.BorrowModule
{
    /// <summary>
    /// Logique d'interaction pour BorrowSpace.xaml
    /// </summary>
    public partial class BorrowSpace : UserControl
    {
        public BorrowSpace()
        {
            InitializeComponent();
            Image_.Source = GetPathHelpers.GetPathImagesource("/UI/Modules/BorrowModule/Assets/image-emprunt.png");
        }
    }
}
