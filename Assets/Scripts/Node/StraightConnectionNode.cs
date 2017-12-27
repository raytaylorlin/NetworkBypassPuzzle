namespace NetworkBypass
{ 
    public class StraightConnectionNode : ConnectionNode
    {
        protected override void ResetRotation()
        {
            base.ResetRotation();
            // 横向
            if (RotateState == ERotateState.Up || RotateState == ERotateState.Down)
            {
                bool hasInput = HasInputFrom(Direction.Left) || HasInputFrom(Direction.Right);
                SetOutput(Direction.Left, hasInput);
                SetOutput(Direction.Right, hasInput);
                SetOutput(Direction.Up, false);
                SetOutput(Direction.Down, false);
                SetActive(hasInput);
            }
            // 纵向
            else
            {
                bool hasInput = HasInputFrom(Direction.Up) || HasInputFrom(Direction.Down);
                SetOutput(Direction.Left, false);
                SetOutput(Direction.Right, false);
                SetOutput(Direction.Up, hasInput);
                SetOutput(Direction.Down, hasInput);
                SetActive(hasInput);
            }
        }
    }
}