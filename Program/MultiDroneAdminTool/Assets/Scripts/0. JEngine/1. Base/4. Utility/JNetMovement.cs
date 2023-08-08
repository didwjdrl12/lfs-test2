using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JNetMovement
    //      ��Ʈ��ũ���� ���ŵ� ��ǥ �� ȸ������ �����Ͽ� �ǽð� ��ǥ ���� ����ȭ�Ѵ�.
    //
    //
    //  ������ ���� ������ ���� �ɼ��� �����ȴ�.
    //
    //      1. Interpolation Type
    //          Time
    //          Speed
    //      2. Rotation Type
    //          None
    //          PositionBase
    //          Angle
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JNetMovement : JComponent
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Nested Class
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [eunm] Interpolation Type
        public enum eInterpolationType
        {
            Time, Speed, None,
        }
        #endregion

        #region [eunm] Rotation Type
        public enum eRotationType
        {
            None, PositionBase, Angle,
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ����
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [����] Config
        public eInterpolationType _interp_type;
        public eRotationType _rotation_type;
        public float _sync_duration = 0.2f;
        public float _move_speed;
        public bool _fixed_update_timer;
        #endregion

        #region [����] Time
        private long _sync_pos_time;
        private long _sync_rot_time;
        private bool _init_value;
        #endregion

        #region [����] Position
        protected Vector3 _sync_position;
        protected Vector3 _prev_sync_position;
        protected Vector3 _last_rot_check_position;
        #endregion

        #region [����] Rotation
        protected Quaternion _sync_rotation;
        protected Quaternion _prev_sync_rotation;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] SyncPosition/SyncRotation
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vector3 SyncPosition
        {
            get { return _sync_position; }
            set { SetSyncPosition(value); }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Quaternion SyncRotation
        {
            get { return _sync_rotation; }
            set { SetSyncRotation(value); }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float SyncAngle
        {
            get { return _sync_rotation.eulerAngles.y; }
            set { SetSyncRotation(Quaternion.Euler(0f, value, 0f)); }
        }
        #endregion

        #region [Property] Position        
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vector3 Position
        {
#if NET_SERVER
            get; set;
#else
            get { return transform.localPosition; }
            set { transform.localPosition = value; }
#endif
        }
        #endregion

        #region [Property] Rotation
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Quaternion Rotation
        {
#if NET_SERVER
            get; set;
#else
            get { return transform.localRotation; }
            set { transform.localRotation = value; }
#endif
        }
        #endregion

        #region [Property] RotationAngle
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual float RotationAngle
        {
            get { return Rotation.eulerAngles.y; }
            set { Rotation = Quaternion.Euler(0f, value, 0f); }
        }
        #endregion
        
        #region [Property] Direction/Right/Up
#if NET_SERVER
        public virtual Vector3 Forward     { get; set; }
        public virtual Vector3 Right       { get; set; }
        public virtual Vector3 Up          { get; set; }
#else
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual Vector3 Forward
        {
            get { return transform.forward; }
            set { Rotation = Quaternion.LookRotation(value); }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual Vector3 Right
        {
            get { return transform.right; }
        }
        public virtual Vector3 Up
        {
            get { return transform.up; }
        }
#endif

        public virtual Vector3 Left { get { return -Right; } }
        public virtual Vector3 Back { get { return -Forward; } }
        public virtual Vector3 Down { get { return -Up; } }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // �⺻ �Լ�
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [�ʱ�ȭ] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Start()
        {
            if (!_init_value)
                Reset(Position, Rotation, false);
        }
        #endregion

        #region [�ʱ�ȭ] Reset
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset(Vector3 sync_pos, Quaternion sync_rot, bool reset_transform = true)
        {
            _prev_sync_position = _last_rot_check_position = _sync_position = sync_pos;
            _sync_rotation = sync_rot;
            _init_value = true;

            if(reset_transform)
            {
                Position = sync_pos;
                Rotation = _sync_rotation;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset()
        {
            Reset(SyncPosition, Rotation, false);
        }
        #endregion

        #region [SyncTransform] Transform
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetSyncTransform(Vector3 sync_pos, Quaternion sync_rot, bool force_move = false)
        {
            SetSyncPosition(sync_pos, force_move);
            SetSyncRotation(sync_rot, force_move);
        }
        #endregion

        #region [SyncTransform] Position
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetSyncPosition(Vector3 sync_pos, bool force_move = false)
        {
            _prev_sync_position = Position;
            _sync_position = sync_pos;
            _sync_pos_time = JTimer.GetCurrentTick();

            if (force_move)
                Position = _prev_sync_position = sync_pos;
        }
        #endregion

        #region [SyncTransform] Rotation
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetSyncRotation(Quaternion sync_rot, bool force_rot = false)
        {
            _prev_sync_rotation = Rotation;
            _sync_rotation = sync_rot;
            _sync_rot_time = JTimer.GetCurrentTick();

            if (force_rot)
                Rotation = _prev_sync_rotation = _sync_rotation;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetSyncAngle(float sync_angle, bool force_rot = false)
        {
            _prev_sync_rotation = Rotation;
            _sync_rotation = Quaternion.Euler(0f, sync_angle, 0f);
            _sync_rot_time = JTimer.GetCurrentTick();

            if (force_rot)
                Rotation = _prev_sync_rotation = _sync_rotation;
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ������Ʈ 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [������Ʈ] ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Update()
        {
            var deltaTime = _fixed_update_timer ? Time.fixedDeltaTime : Time.deltaTime;

            updatePositionMovement(deltaTime);
            updateRotationMovement(deltaTime);
        }
        #endregion

        #region [������Ʈ] Position Movement
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void updatePositionMovement(float deltaTime)
        {
            if (_interp_type == eInterpolationType.Speed)
            {
                var distance = Vector3.Distance(_sync_position, _prev_sync_position);
                if (distance > 3f)
                    _prev_sync_position = Position = _sync_position; // �ʹ� �ָ� ���� �̵�
                MoveTowards(_sync_position, _move_speed * deltaTime);
            }
            else if (_interp_type == eInterpolationType.Time) // �ð� ����̵�
            {
                var elasped_time = JTimer.GetElaspedTime(_sync_pos_time) / _sync_duration;
                if (elasped_time > 1f)
                {
                    Position = _prev_sync_position = _sync_position;
                    elasped_time = 1f;
                }
                else
                {
                    Position = Vector3.Lerp(_prev_sync_position, _sync_position, elasped_time);
                }
            }
        }
        #endregion

        #region [������Ʈ] Rotation Movement
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void updateRotationMovement(float deltaTime)
        {
            if (_rotation_type == eRotationType.PositionBase)
            {
                //_position = Vector3.Lerp(_position, _sync_position_private, Time.deltaTime * 10f);
                //var tarRot = Quaternion.AngleAxis(_sync_rotation, Vector3.up);
                var tar_dir = _sync_position - _last_rot_check_position;
                if (tar_dir.magnitude > 5)
                {
                    _sync_rotation = Quaternion.LookRotation(tar_dir, Vector3.up);
                    _last_rot_check_position = _sync_position;
                }
                Rotation = Quaternion.Slerp(Rotation, _sync_rotation, deltaTime * 10f);
            }
            else if (_rotation_type == eRotationType.Angle)
            {
                var tar_rot = Quaternion.AngleAxis(SyncAngle, Vector3.up);
                Rotation = Quaternion.Slerp(Rotation, tar_rot, deltaTime * 20f);
            }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Movement 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [�̵�] ������ ���� �˻�
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool IsArriveDestination(Vector3 finalDest, Vector3 nextPos, float checkDist = 0.1f)
        {
            Vector3 tarDir = finalDest - Position;
            float distance = tarDir.magnitude;
            // �Ÿ��� ������ ����
            if (distance < checkDist)
                return true;

            Vector3 nextPosDir = finalDest - nextPos;

            // �̵������ �ݴ� ������ ���ϸ� ����
            if (Vector3.Angle(tarDir, nextPosDir) > 90.0f)
                return true;

            return false;
        }
        #endregion

        #region [�̵�] ��ǥ �������� �̵�
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool MoveTowards(Vector3 target, float movement, float checkDist = 0.1f, bool lookAt = false)
        {
            var dir = (target - Position);
            dir.Normalize();
            var nextPos = Position + (dir * movement);

            if (IsArriveDestination(target, nextPos, checkDist))
            {
                Position = target;
                return true;
            }

#if NET_SERVER
            //if (lookAt)
            //    _rotation = Quaternion.LookRotation(dir); // todo : 
#else
            if (lookAt)
                Rotation = Quaternion.LookRotation(dir);
#endif
            Position = nextPos;
            return false;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Factory
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Factory] ����
        //------------------------------------------------------------------------------------------------------------------------------------------------------
#if NET_SERVER
        public static JNetMovement Make(JActor target, eInterpolationType interp_type = eInterpolationType.Time, eRotationType rot_type = eRotationType.None)        
#else
        public static JNetMovement Make(GameObject target, eInterpolationType interp_type = eInterpolationType.Time, eRotationType rot_type = eRotationType.None)
#endif
        {
            var movement = target.AddComponent<JNetMovement>();
            movement._interp_type = interp_type;
            movement._rotation_type = rot_type;
            return movement;
        }
        #endregion


    }

}

