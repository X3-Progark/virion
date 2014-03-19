

using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Virion
{
    /// <summary>
    /// Enum describes the view transition state.
    /// </summary>
    public enum ViewState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }


    /// <summary>
    /// A view is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as views.
    /// </summary>
    public abstract class GameView
    {
        /// <summary>
        /// Normally when one view is brought up over the top of another,
        /// the first view will transition off to make room for the new
        /// one. This property indicates whether the view is only a small
        /// popup, in which case views underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        bool isPopup = false;


        /// <summary>
        /// Indicates how long the view takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;


        /// <summary>
        /// Indicates how long the view takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;


        /// <summary>
        /// Gets the current position of the view transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;


        /// <summary>
        /// Gets the current alpha of the view transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }


        /// <summary>
        /// Gets the current view transition state.
        /// </summary>
        public ViewState ViewState
        {
            get { return viewState; }
            protected set { viewState = value; }
        }

        ViewState viewState = ViewState.TransitionOn;


        /// <summary>
        /// There are two possible reasons why a view might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// view that is on top of it, or it could be going away for good.
        /// This property indicates whether the view is exiting for real:
        /// if set, the view will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        bool isExiting = false;


        /// <summary>
        /// Checks whether this view is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherViewHasFocus &&
                       (viewState == ViewState.TransitionOn ||
                        viewState == ViewState.Active);
            }
        }

        bool otherViewHasFocus;


        /// <summary>
        /// Gets the manager that this view belongs to.
        /// </summary>
        public ViewManager ViewManager
        {
            get { return viewManager; }
            internal set { viewManager = value; }
        }

        ViewManager viewManager;


        /// <summary>
        /// Gets the index of the player who is currently controlling this view,
        /// or null if it is accepting input from any player. This is used to lock
        /// the game to a specific player profile. The main menu responds to input
        /// from any connected gamepad, but whichever player makes a selection from
        /// this menu is given control over all subsequent views, so other gamepads
        /// are inactive until the controlling player returns to the main menu.
        /// </summary>
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }

        PlayerIndex? controllingPlayer;




        /// <summary>
        /// Unload content for the view. Called when the view is removed from the view manager.
        /// </summary>
        public virtual void Unload() { }

        /// <summary>
        /// Unload content for the view. Called when the view is removed from the view manager.
        /// </summary>
        public virtual void Activate(Boolean boolean) { }

        /// <summary>
        /// Allows the view to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the view
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherViewHasFocus, bool coveredByOtherView)
        {
            this.otherViewHasFocus = otherViewHasFocus;

            if (isExiting)
            {
                // If the view is going away to die, it should transition off.
                viewState = ViewState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the view.
                    ViewManager.RemoveView(this);
                }
            }
            else if (coveredByOtherView)
            {
                // If the view is covered by another, it should transition off.
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    viewState = ViewState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    viewState = ViewState.Hidden;
                }
            }
            else
            {
                // Otherwise the view should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    viewState = ViewState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    viewState = ViewState.Active;
                }
            }
        }


        /// <summary>
        /// Helper for updating the view transition position.
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        /// <summary>
        /// Allows the view to handle user input. Unlike Update, this method
        /// is only called when the view is active, and not when some other
        /// view has taken the focus.
        /// </summary>
        public virtual void HandleInput(GameTime gameTime, InputState input) { }


        /// <summary>
        /// This is called when the view should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime) { }


        /// <summary>
        /// Tells the view to go away. Unlike ViewManager.RemoveView, which
        /// instantly kills the view, this method respects the transition timings
        /// and will give the view a chance to gradually transition off.
        /// </summary>
        public void ExitView()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the view has a zero transition time, remove it immediately.
                ViewManager.RemoveView(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                isExiting = true;
            }
        }
    }
}
