using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Spritehand.FarseerHelper;
using Spritehand.PhysicsBehaviors;
using Point = Microsoft.Xna.Framework.Point;

namespace slHackathonGame.Pages
{
    public partial class GamePage
    {
        private readonly List<DistanceJoint> _joints;
        private PhysicsSprite _player;
        private System.Windows.Point _startDrag;
        private Ellipse _finger;

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

    

       

        private void PhysicsControllerInitialized(object source)
        {
            //get reference to controller
            var physicsController = source as PhysicsControllerMain;


            _player = physicsController.PhysicsObjects["player"];
            _player.Collision += _player_Collision;
     

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

        void _player_Collision(PhysicsSprite source, string collidedWith)
        {
            MessageBox.Show("Show body touched the player");
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

       


        private void GestureListenerDragStarted(object sender, Microsoft.Phone.Controls.DragStartedGestureEventArgs e)
        {
            //get drag start
            _startDrag = e.GetPosition(LayoutRoot);
             var physicsController =
             LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;

            if(_finger == null)

            {
                //create finger ellipse
             var _physicsController = LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;
             _finger = new Ellipse { Name = "finger", Stroke = new SolidColorBrush(Colors.Red), Width = 50, Height = 50,StrokeThickness = 2};
             var behaviorCollection = Interaction.GetBehaviors(_finger);
             behaviorCollection.Add(new PhysicsObjectBehavior() { BoundaryElement = "finger", IsBullet = true });
             _physicsController.AddPhysicsBody(_finger.GetValue(PhysicsObjectMain.PhysicsObjectProperty) as PhysicsObjectMain);
              
              
            }
            



  
           
           
        
        }

        private void GestureListenerDragCompleted(object sender, Microsoft.Phone.Controls.DragCompletedGestureEventArgs e)
        {
            if(_finger != null)
            {
                _finger.Stroke = new SolidColorBrush(Colors.Transparent);
            }
            return;
            var physicsController =
             LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;

           /* MessageBox.Show(
                String.Format(
                    "Silverlight Touchpoint: {0}\n Farseer Vector:{1}\n Farseer ScreenTopLeft: {2}\nFarseer ScreenBottomRight:{3}\nFarseer WorldTopLeft{4}\nFarseer WorldBottomRight{5}", e.ManipulationOrigin.ToString(), physicsController.ScreenToWorld(e).ToString(), BoundaryHelperBox2d.ScreenTopLeft, BoundaryHelperBox2d.ScreenBottomRight, BoundaryHelperBox2d.WorldTopLeft, BoundaryHelperBox2d.WorldBottomRight));*/


            var pt = new System.Windows.Point((int)e.GetPosition(LayoutRoot).X, (int)e.GetPosition(LayoutRoot).Y);

            var letGoVector = physicsController.ScreenToWorld(pt);
            MessageBox.Show(letGoVector.ToString());

            e.Handled = true;
        }

        private void GestureListenerDragDelta(object sender, Microsoft.Phone.Controls.DragDeltaGestureEventArgs e)
        {

            //move finger
            var _physicsController = LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;
            var finger = _physicsController.PhysicsObjects["finger"];

            finger.Position = new Vector2((int)e.GetPosition(LayoutRoot).X, (int)e.GetPosition(LayoutRoot).Y);



        }
    }
}