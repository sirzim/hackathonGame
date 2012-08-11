using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Spritehand.FarseerHelper;
using Spritehand.PhysicsBehaviors;
using slHackathonGame.UserControls;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace slHackathonGame.Pages
{
    public partial class GamePage
    {
        private readonly List<DistanceJoint> _joints;
        private bool _touchingPlayer;
        private Ellipse _finger;
        private PhysicsSprite _player;
        private Vector2 _startDrag;
        private Line _dragLine;

        private static Random random = new Random();
        private enum Side
        {
            Left = 0,
            Right = 1,
            size = 2
        }
        private PhysicsControllerMain _physicsController;

        public GamePage()
        {
            InitializeComponent();
            Loaded += GamePageLoaded;
            _joints = new List<DistanceJoint>();


        
        }


        private void GamePageLoaded(object sender, RoutedEventArgs e)
        {
            //get reference to physics controller
            _physicsController =
                LayoutRoot.GetValue(PhysicsControllerMain.PhysicsControllerProperty) as PhysicsControllerMain;


            //hook up initilized event
            _physicsController.Initialized += PhysicsControllerInitialized;


            //setup finger
            _finger = new Ellipse
                          {
                              Name = "finger",
                              Height = 50,
                              Width = 50,
                              StrokeThickness = 3,
                              Stroke = new SolidColorBrush(Colors.Red)
                          };

            // Spawn initial branches
            for (int i = 0; i < (random.Next(2) + 4); i++)
            {
                spawnBranch((Side)random.Next(2), random.Next(500));
            }


        }

        private void spawnBranch()
        {
            spawnBranch((Side)random.Next((int)Side.size), 0);
        }

        private void spawnBranch(Side side)
        {
            spawnBranch(side, 0);
        }

        private void spawnBranch(int y)
        {
            spawnBranch((Side)random.Next((int)Side.size), y);
        }

        private void spawnBranch(Side side, int y)
        {
            spawnBranch(
                side,
                y,
                80 + random.Next(200),
                30 + random.Next(10)
            );
        }

        private void spawnBranch(Side side, int y, int width, int height)
        {
            Branch branch = new Branch()
            {
                Width = width,
                Height = height,
                Name = (Guid.NewGuid()).ToString()
            };

            branch.Location = new System.Windows.Point(
                side == Side.Left ?
                    (branch.Width / 2) :
                    this.LayoutRoot.ActualWidth - (branch.Width / 2),
                y
            );

            this.LayoutRoot.Children.Add(branch);
            //LayoutRoot.Children
        }

        private void PhysicsControllerInitialized(object source)
        {
           


            _player = _physicsController.PhysicsObjects["player"];
            _physicsController.TimerLoop += PhysicsControllerTimerLoop;



            //var branch = _physicsController.PhysicsObjects["branch"];
            //branch.DoubleTap += BranchDoubleTap;



        }

        void BranchDoubleTap(object sender, GestureEventArgs e)
        {
            //var obj = sender as PhysicsSprite;
            SetupDistanceJoint(sender);
        }

   

     

        void PhysicsControllerTimerLoop(object source)
        {

          //  var finger = _physicsController.PhysicsObjects["branch"];
         
        }


   
        private void SetupReleaseTarget(object sender, PhysicsControllerMain physMain, ManipulationCompletedEventArgs e)
        {
            //remove old joints
            if (_joints.Count > 0)
            {
                foreach (DistanceJoint jnt in _joints)
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


            var letgopoint = new Point((int) e.ManipulationOrigin.X + (int) e.TotalManipulation.Translation.X,
                                       (int) e.ManipulationOrigin.Y + (int) e.TotalManipulation.Translation.Y);

            Body testBody = BodyFactory.CreateCircle(physMain.Simulator, 25, 1, physMain.ScreenToWorld(letgopoint));

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

        private void SetupDistanceJoint(object sender)
        {
            //remove old joints
            if (_joints.Count > 0)
            {
                foreach (DistanceJoint jnt in _joints)
                {
                    _physicsController.DeleteObject(jnt);
                }
            }

            const float breakpoint = 1000f;

            //translate coordinates relative to screen
            var touchedObject = sender as PhysicsSprite;
            //var list = BoundaryHelper.GetPointsForElement(touchedObject.uiElement, e.ManipulationContainer, false);

            //get reference to player
            PhysicsSprite player = _physicsController.PhysicsObjects["player"];


            //create distance joint between the two
            DistanceJoint joint = JointFactory.CreateDistanceJoint(_physicsController.Simulator, player.BodyObject,
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
                                      _physicsController.DeleteObject(joint);
                                      return;
                                  }

                                  //reduce distance
                                  if (joint.Length <= 0f)
                                  {

                                      timer.Stop();
                                      _physicsController.DeleteObject(joint);
                                  }
                                  joint.Length -= .1f;
                              };
            timer.Start();
        }

        private void joint_Broke(Joint arg1, float arg2)
        {
            arg1.Enabled = false;
        }


        private void LayoutRootManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
       
            BehaviorCollection behaviorCollection = Interaction.GetBehaviors(_finger);
            if (behaviorCollection.Count == 0)
            {
                behaviorCollection.Add(new PhysicsObjectBehavior
                                           {BoundaryElement = "finger",IsStatic =true,IsSensor = true});
                _physicsController.AddPhysicsBody(
                    _finger.GetValue(PhysicsObjectMain.PhysicsObjectProperty) as PhysicsObjectMain);
            }


            PhysicsSprite finger = _physicsController.PhysicsObjects["finger"];
            

            finger.Collision += FingerCollision;
            finger.BodyObject.OnSeparation += BodyObject_OnSeparation;

            //finger phys object already created
            if (finger != null)
            {
                finger.Position = new Vector2((float) (e.ManipulationOrigin.X),
                                              (float) (e.ManipulationOrigin.Y));
            }


          
            

          


            e.Handled = true;
        }

        void BodyObject_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
           if(_player.BodyObject.FixtureList.Contains(fixtureA) || _player.BodyObject.FixtureList.Contains(fixtureB))
           {
               target.Stroke =  new SolidColorBrush(Colors.Transparent);
               _touchingPlayer = false;
           }
        }

        private void FingerCollision(PhysicsSprite source, string collidedWith)
        {
            Debug.WriteLine(collidedWith);
            if (collidedWith != "player") return;
            _touchingPlayer = true;
            //Debug.WriteLine("touchingPlayer");


            target.Stroke = new SolidColorBrush(Colors.Red);
            target.SetValue(Canvas.TopProperty, _player.Position.Y - (target.Height /2));
            target.SetValue(Canvas.LeftProperty, _player.Position.X - (target.Width / 2));
        }

        private void LayoutRootManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            
            
        }

        private void LayoutRootManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            
            
            PhysicsSprite finger = _physicsController.PhysicsObjects["finger"];
            

            double x = finger.Position.X + e.DeltaManipulation.Translation.X;
            double y = finger.Position.Y + e.DeltaManipulation.Translation.Y;

            finger.Position = new Vector2((float) x,
                                          (float) y);
            _startDrag = finger.Position;


            if (_dragLine == null)
            {
                _dragLine = new Line
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 254, 192, 200)),
                    StrokeThickness = 4,
                };
            }

            //set new line properties
            _dragLine.StrokeThickness = 2;
            _dragLine.X1 = _startDrag.X;
            _dragLine.X2 = _startDrag.X - e.DeltaManipulation.Translation.X;
            _dragLine.Y1 = _startDrag.Y;
            _dragLine.Y2 = _startDrag.Y - e.DeltaManipulation.Translation.Y;
            _dragLine.Name = "line";

            if (!LayoutRoot.Children.Contains(_dragLine))
            {
                LayoutRoot.Children.Add(_dragLine);
            }

            
        
          


            e.Handled = true;
        }


       
        

    }
}