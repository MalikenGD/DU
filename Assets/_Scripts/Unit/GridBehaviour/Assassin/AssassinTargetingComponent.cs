namespace _Scripts.Unit.GridBehaviour
{
    public class AssassinTargetingComponent : TargetingComponent
    {
        public bool BehindTarget()
        {
            return transform.position.y < _currentTarget.transform.position.y;
        }
    }
}