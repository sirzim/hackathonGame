using System.Windows;
using Spritehand.FarseerHelper;

namespace slHackathonGame.Pages
{
public partial class GamePage
    {
        
        public GamePage()
        {
            InitializeComponent();
            Loaded += GamePageLoaded;
            
        }

        void GamePageLoaded(object sender, RoutedEventArgs e)
        {
            //get reference to physics controller
            var physicsController = LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;
            //hook up initilized event
            physicsController.Initialized += PhysicsControllerInitialized;
           
        }

        void PhysicsControllerInitialized(object source)
        {
            //get reference to controller
            var physicsController = source as PhysicsControllerMain;

            //get reference to branch
            if (physicsController != null)
            {
                var bbox = physicsController.PhysicsObjects["bbox"];

                //hook up collision event
                bbox.Collision += BranchCollision;
            }
        }

        void BranchCollision(PhysicsSprite source, string collidedWith)
        {
            //MessageBox.Show("Hit branch");
        }
    }
}