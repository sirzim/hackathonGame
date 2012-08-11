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
                //get reference to bounding box
                var bbox = physicsController.PhysicsObjects["bbox"];

                //hook up collision event
                bbox.Collision += BranchCollision;
            }

            //get location they clicked on branch
            var branch = physicsController.PhysicsObjects["branch"];
            branch.ManipulationCompleted += branch_ManipulationCompleted;


            //setup a joint to test physics
            
           // SetupPulleyJoint(physicsController);
        }

        void BranchCollision(PhysicsSprite source, string collidedWith)
        {
            //MessageBox.Show("Hit branch");
        }

        void SetupPulleyJoint(PhysicsControllerMain physMain)
        {

            //get reference to player
            var player = physMain.PhysicsObjects["player"];
      


        }

        void branch_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            //translate coordinates relative to screen
            var physObject = sender as PhysicsSprite;
            MessageBox.Show(physObject.Position.ToString());

            MessageBox.Show(e.ManipulationOrigin.ToString());
            
            
        }
    }
}