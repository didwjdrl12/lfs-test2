using J2y.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JActorTransform : JComponent
    //
    //      1. 
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JActorTransform : JComponent
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] 0. �⺻����
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] ��ǥ, ����
#if NET_SERVER
        [JSerializable] public virtual Vector3 _position    { get; set; }
        [JSerializable] public virtual float _rotation      { get; set; }
        [JSerializable] public virtual Vector3 _direction   { get; set; }
        [JSerializable] public virtual Vector3 _right       { get; set; }
        [JSerializable] public virtual Vector3 _up          { get; set; }
#else
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual Vector3 _position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual Quaternion _rotation
        {
            get { return transform.rotation; }
            set { transform.rotation = value; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual Vector3 _direction
        {
            get { return transform.forward; }
            set { transform.rotation = Quaternion.LookRotation(value); }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual Vector3 _right
        {
            get { return transform.right; }
        }
        public virtual Vector3 _up
        {
            get { return transform.up; }
        }
#endif
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // �̵�, ȸ��
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [�̵�] ������ ���� �˻�
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool IsArriveDestination(Vector3 finalDest, Vector3 nextPos, float checkDist = 0.1f)
	    {
		    Vector3 tarDir = finalDest - _position;
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
		    var dir = (target - _position);
		    dir.Normalize();
		    var nextPos = _position + (dir * movement);

		    if (IsArriveDestination(target, nextPos, checkDist))
		    {
			    _position = target;
			    return true;
		    }

#if NET_SERVER
            //if (lookAt)
            //    _rotation = Quaternion.LookRotation(dir); // todo : 
#else
            if (lookAt)
                _rotation = Quaternion.LookRotation(dir);
#endif
            _position = nextPos;
		    return false;
	    }
	    #endregion

	    #region [�̵�] ��ǥ ���� �ٶ󺸱�
	    //------------------------------------------------------------------------------------------------------------------------------------------------------
	    public void RotateTowards(Vector3 target, float rotSpeed)
	    {
		    var direction = target - _position;
		    direction.y = 0;
		    if (direction.magnitude < 0.1)
			    return;

            // Rotate towards the target
#if NET_SERVER
            //_rotation = Quaternion.Slerp(_rotation, Quaternion.LookRotation(direction), rotSpeed); // todo : 
#else
            _rotation = Quaternion.Slerp(_rotation, Quaternion.LookRotation(direction), rotSpeed);
#endif
            
            //?? transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        #endregion


        #region [�̵�] [�ڷ�ƾ] Ư�� �������� �̵�
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public IEnumerator coroutine_moveTo(Vector3 targetPos, float moveSpeed, float checkDist, bool lookAt = false)
        //{
        //	var duration = 10f / moveSpeed;
        //	var dir = (targetPos - _position);
        //	dir.Normalize();
        //	var finalTarget = targetPos - (dir * checkDist);


        //	//while (true)
        //	//{
        //	//    if (MoveTowards(finalTarget, Time.deltaTime * moveSpeed, 0.1f, lookAt))
        //	//        break;
        //	//    yield return null;
        //	//}

        //	if (lookAt)
        //		transform.rotation = Quaternion.LookRotation(dir);

        //	JUtil.MoveTo(gameObject, finalTarget, duration, "easeInOutCirc");
        //	yield return new WaitForSeconds(duration);
        //}
        #endregion


    }

}