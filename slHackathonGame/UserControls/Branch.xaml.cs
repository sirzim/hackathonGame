using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Spritehand.FarseerHelper;
using Point = System.Windows.Point;

namespace slHackathonGame.UserControls
{
    public partial class Branch : UserControl
    {
        public Point Location { get; set; }
        public Branch()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(BranchLoaded);
        }

        void BranchLoaded(object sender, RoutedEventArgs e)
        {
            var physicsController =
                Parent.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;
            if(physicsController != null)
            physicsController.Initialized += PhysicsControllerInitialized;
        }

        void PhysicsControllerInitialized(object source)
        {
            var physicsController = source as PhysicsControllerMain;
            var physObj = physicsController.PhysicsObjects[this.Name];
            physObj.Position = new Vector2((float)Location.X, (float)Location.Y);
            physicsController.Initialized -= PhysicsControllerInitialized;
        }
    }
}
