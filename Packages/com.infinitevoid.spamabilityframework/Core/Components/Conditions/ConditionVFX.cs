using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.Conditions
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ConditionVFX : MonoBehaviour
    {
        [SerializeField] private bool _fixedRotation;
        
        private Quaternion _initialRotation;
        private Transform _transform;
        private ParticleSystem _particleSystem;
        private Transform _followTarget;
        private Vector3 _offset;


        private void Awake()
        {
            _transform = transform;
            _initialRotation = _transform.rotation;
            this.enabled = false;
            TryGetComponent(out _particleSystem);
        }

        private void LateUpdate()
        {
            if(_fixedRotation)
                _transform.rotation = _initialRotation;
            if (_followTarget)
                _transform.position = _followTarget.position + _offset;
        }

        public void Stop()
        {
            if(_particleSystem)
                _particleSystem.Stop();
            _followTarget = null;
        }
        
        public void Restart()
        {
            if(_particleSystem)
                _particleSystem.Stop();
            _particleSystem.Play();
        }

        /// <summary>
        /// Sets the given transform as a target to follow.
        /// Note that the actual position of the VFX should be set before calling follow,
        /// as the offset between the VFX's current position and the target will be used as
        /// an offset to the followed transform's position.
        /// </summary>
        /// <param name="transformToFollow"></param>
        public void FollowTarget(Transform transformToFollow)
        {
            _followTarget = transformToFollow;
            _offset = _transform.position - transformToFollow.position;
            this.enabled = true;
        }
    }
}
