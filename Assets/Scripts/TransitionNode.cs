namespace NetworkBypass
{
	public class TransitionNode : NetworkNode
	{
		protected override void Init()
		{
			base.Init();
			Sprite.gameObject.SetActive(false);
		}
		
		public override void SetInput(Direction from, bool isActive)
		{
			base.SetInput(from, isActive);
			// 仅转发输入
			for (int i = 0; i < NeighborNum; i++)
			{
				bool hasOutput = HasInput && GetNeighbor(i) != null;
				SetOutput((Direction) i, hasOutput);
			}
		}
	}
}