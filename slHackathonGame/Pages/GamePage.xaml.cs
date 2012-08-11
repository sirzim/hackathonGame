using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Spritehand.FarseerHelper;
using Point = Microsoft.Xna.Framework.Point;

namespace slHackathonGame.Pages
{
    public partial class GamePage
    {
        private readonly List<DistanceJoint> _joints;
        private PhysicsSprite _player;


        public GamePage()
        {
            InitializeComponent();
            Loaded += GamePageLoaded;
            _joints = new List<DistanceJoint>();
        }

       

        private void GamePageLoaded(object sender, RoutedEventArgs e)
        {
            //get reference to physics controller
            var physicsController =
                LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;
            //hook up initilized event
            physicsController.Initialized += PhysicsControllerInitialized;
          
        }

        void _player_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        { 
            
        var physicsController =
                LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;
            SetupReleaseTarget(sender,physicsController,e);
        }

        void _player_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
          
        }

        private void PhysicsControllerInitialized(object source)
        {
            //get reference to controller
            var physicsController = source as PhysicsControllerMain;


            _player = physicsController.PhysicsObjects["player"];
            _player.ManipulationStarted += _player_ManipulationStarted;
         //   _player.ManipulationCompleted += _player_ManipulationCompleted;

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

        private void SetupReleaseTarget(object sender, PhysicsControllerMain physMain, ManipulationCompletedEventArgs e)
        {

            //remove old joints
            if (_joints.Count > 0)
            {
                foreach (var jnt in _joints)
                {
                    physMain.DeleteObject(jnt);
                }
            }

            const float breakpoint = 1000f;

            //translate coordinates relative to screen
            var touchedObject = sender as PhysicsSprite;
            //var list = BoundaryHelper.GetPointsForElement(touchedObject.uiElement, e.ManipulationContainer, false);

            //get reference to player
            PhysicsSprite player = physMain.PhysicsObjects["player"];


            var letgopoint = new System.Windows.Point((int)e.ManipulationOrigin.X + (int)e.TotalManipulation.Translation.X,
                                       (int)e.ManipulationOrigin.Y + (int)e.TotalManipulation.Translation.Y);

            var testBody = BodyFactory.CreateCircle(physMain.Simulator, 25, 1, physMain.ScreenToWorld(letgopoint));

            //create distance joint between the two
            DistanceJoint joint = JointFactory.CreateDistanceJoint(physMain.Simulator, player.BodyObject,
                                                                   testBody, Vector2.Zero, Vector2.Zero);

            joint.Frequency = 4.0f;
            joint.DampingRatio = .5f;
            joint.Breakpoint = breakpoint;
            joint.CollideConnected = true;
            joint.Broke += joint_Broke;
            _joints.Add(joint);

            //create tounge


            //timer
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };

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

                    timer.Stop();
                }
                joint.Length -= .1f;
            };
            timer.Start();
        }

        private void SetupDistanceJoint(object sender, PhysicsControllerMain physMain, ManipulationCompletedEventArgs e)
        {

            //remove old joints
            if(_joints.Count > 0)
            {
                foreach (var jnt in _joints)
                {
                    physMain.DeleteObject(jnt);
                }
            }

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
            _joints.Add(joint);

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

        private void LayoutRoot_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
           
            var physicsController =
        LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;

            MessageBox.Show(
                String.Format(
                    "Silverlight Touchpoint: {0}\n Farseer Vector:{1}\n Farseer ScreenTopLeft: {2}\nFarseer ScreenBottomRight:{3}\nFarseer WorldTopLeft{4}\nFarseer WorldBottomRight{5}",e.ManipulationOrigin.ToString(),physicsController.ScreenToWorld(e.ManipulationOrigin).ToString(),BoundaryHelperBox2d.ScreenTopLeft,BoundaryHelperBox2d.ScreenBottomRight,BoundaryHelperBox2d.WorldTopLeft,BoundaryHelperBox2d.WorldBottomRight));


            e.Handled = true;
        }
    }
}