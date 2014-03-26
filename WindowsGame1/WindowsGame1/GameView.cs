

using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Virion
{
    
    /// Enum describes the view transition state.
    
    public enum ViewState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    public abstract class GameView
    {

        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        bool isPopup = false;


        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;


        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;


        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;


        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }


        public ViewState ViewState
        {
            get { return viewState; }
            protected set { viewState = value; }
        }

        ViewState viewState = ViewState.TransitionOn;


        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        bool isExiting = false;


        
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


        public ViewManager ViewManager
        {
            get { return viewManager; }
            internal set { viewManager = value; }
        }

        ViewManager viewManager;


        
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }

        PlayerIndex? controllingPlayer;



        public virtual void Unload() { }

        public virtual void Activate(Boolean boolean) { }

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

        public virtual void HandleInput(GameTime gameTime, InputState input) { }

        public virtual void Draw(GameTime gameTime) { }

        public void ExitView()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                ViewManager.RemoveView(this);
            }
            else
            {
                isExiting = true;
            }
        }
    }
}
