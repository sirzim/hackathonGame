using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
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

       

        private void GamePageLoaded(object sender, RoutedEventArgs e)
        {
            //get reference to physics controller
            var physicsController =
                LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;
            //hook up initilized event
            physicsController.Initialized += PhysicsControllerInitialized;
        }

        private void PhysicsControllerInitialized(object source)
        {
            //get reference to controller
            var physicsController = source as PhysicsControllerMain;


            //get reference to branch
            if (physicsController != null)
            {
                //get reference to bounding box
                PhysicsSprite bbox = physicsController.PhysicsObjects["bbox"];

                //hook up collision event
                bbox.Collision += BranchCollision;
            }

            //get location they clicked on branch
            PhysicsSprite branch = physicsController.PhysicsObjects["branch"];
            PhysicsSprite branch1 = physicsController.PhysicsObjects["branch1"];
            branch1.ManipulationCompleted += (sender, e) => SetupDistanceJoint(sender, physicsController, e);
            branch.ManipulationCompleted += (sender, e) => SetupDistanceJoint(sender, physicsController, e);
        }

        private void BranchCollision(PhysicsSprite source, string collidedWith)
        {
            //MessageBox.Show("Hit branch");
        }

        private void SetupDistanceJoint(object sender, PhysicsControllerMain physMain, ManipulationCompletedEventArgs e)
        {
            const float breakpoint = 1000f;

            //translate coordinates relative to screen
            var touchedObject = sender as PhysicsSprite;
            //var list = BoundaryHelper.GetPointsForElement(touchedObject.uiElement, e.ManipulationContainer, false);

            //get reference to player
            PhysicsSprite player = physMain.PhysicsObjects["player"];


            //create distance joint between the two
            DistanceJoint joint = JointFactory.CreateDistanceJoint(physMain.Simulator, player.BodyObject,
                                                                   touchedObject.BodyObject, Vector2.Zero, Vector2.Zero);

            joint.Frequency = 4.0f;
            joint.DampingRatio = .5f;
            joint.Breakpoint = breakpoint;
            joint.CollideConnected = true;
            joint.Broke += joint_Broke;

            //create tounge
            

            //timer
            var timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(33)};

            timer.Tick += delegate
                              {
                                  //joint broke
                                  if (!joint.Enabled)
                                  {
                                      timer.Stop();
                                      physMain.DeleteObject(joint);
                                      return;
                                  }

                                  //reduce distance
                                  if (joint.Length <= 0f)
                                  {
                                      physMain.DeleteObject(joint);
                                      timer.Stop();
                                  }
                                  joint.Length -= .1f;
                              };
            timer.Start();
        }


        private void joint_Broke(Joint arg1, float arg2)
        {
            arg1.Enabled = false;
        }

        private void BranchManipulationCompleted()
        {
            // MessageBox.Show(list[0].ToString());

            // MessageBox.Show(e.ManipulationOrigin.ToString());
        }
    }
}