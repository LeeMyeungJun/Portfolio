using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnvironmentQuery : MonoBehaviour
{
    public struct EQSData
    {
        public Vector3 position;
        public float point;
    }


    static NavMeshPath path;
    static GameObject player = null;
    static GameObject Player 
    { 
        get 
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");
            return player;
        } 
    }

    //1  NavMesh가 갈수있는지 체크 못가면 -10점
    //2 EnemySideRange 안에 있는가+1
    //3 EnemyMeleeRange 안에있으면 지운다 . -1
    //4 포인트랑 가까운순

    static float EQScalculator(Vector3 _pos,GameObject _target,float _searchRange,float _nearRange, NavMeshAgent _agent)
    {

        float eqsValue = 0;
        if(path == null)
            path = new NavMeshPath();
        bool valid = _agent.CalculatePath(_pos, path);
        
        if (valid)//갈수있는 장소라는 의미
        {
            float distance = Vector3.Distance(_pos, _target.transform.position);

            if(_searchRange > distance)
                eqsValue += 1;

            if (_nearRange > distance)
                eqsValue -= 1;

            return eqsValue;
        }
        eqsValue = -5f;

        return eqsValue;

    }

    public static float[] GetEQSPoints(GameObject _target, Vector3[] points, float _searchRange, float _nearRange,NavMeshAgent _agent)
    {
        //Vector3[] positio  = GetPolygonPositionsAngleFunction(pointCount, sideRange);
        float[] result = new float[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            result[i] = EQScalculator(points[i], _target,_searchRange,_nearRange, _agent);
        }

        return result;
    }

    public static EQSData[] GetEqsData(int _pointCount,float _searchRange,float _nearRange,GameObject _target,NavMeshAgent _agent)
    {
        EQSData[] data = new EQSData[_pointCount];
        Vector3[] PointPositions = GetPolygonPositionsAngleFunction(_pointCount, _searchRange,_target);
        float[] points = GetEQSPoints(_target, PointPositions,_searchRange,_nearRange,_agent);
        for (int i = 0; i < _pointCount; i++)
        {
            data[i].position = PointPositions[i];
            data[i].point = points[i];
        }

        return data;
    }

    public static EQSData GetEqsRandomHighPoint(int _pointCount, float _searchRange, float _nearRange, GameObject _target, NavMeshAgent _agent)
    {
        EQSData data = new EQSData();
        EQSData[] eqs = GetEqsData(_pointCount, _searchRange, _nearRange, _target,_agent);
        List<int> indexs = new List<int>();

        float maxPoint = 0;
        for (int i = 0; i < eqs.Length; i++)
        {
            if (maxPoint < eqs[i].point)
                maxPoint = eqs[i].point;
        }


        for (int i = 0; i < eqs.Length; i++)
        {
            if (maxPoint == eqs[i].point)
                indexs.Add(i);
        }

        int randomIndex = 0;
        if(indexs.Count > 0)
        {
            randomIndex = Random.Range(0, indexs.Count);

        }
        else
        {
            data.position = Vector3.zero;
            return data;
        }
        data.position = eqs[randomIndex].position;
        data.point = eqs[randomIndex].point;
        return data;
    }

    public static Vector3[] GetPolygonPositionsAngleFunction(int numberOfPolygonPoint, float radius,GameObject _target)
    {
        List<Vector3> positions = new List<Vector3>();
        float angle = 360f / numberOfPolygonPoint;
        float currentAngle = 0f;

        for (int i = 0; i < numberOfPolygonPoint; i++)
        {
            float radian = Mathf.Deg2Rad * currentAngle;

            float x = Mathf.Cos(radian) * radius;
            float z = Mathf.Sin(radian) * radius;
            currentAngle += angle;

            //player 주변으로변경해야함
            positions.Add(new Vector3(x, 0, z) + Player.transform.transform.position);
        }

        return positions.ToArray();
    }

    //private void OnDrawGizmos()
    //{
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(this.transform.position, searchRange);

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(this.transform.position, nearRange);

        //Gizmos.color = Color.blue;
        //if (pointCount < 3 || testObj == null || !Application.isPlaying)
        //    return;
        //Handles.color = Color.black;
        //Vector3[] PointPositions = GetPolygonPositionsAngleFunction(pointCount, searchRange);
        //float[] floats = GetEQSPoints(testObj,PointPositions);
   
        //GUIStyle style = new GUIStyle();
        //style.fontSize = 48;
        //for (int i = 0; i < pointCount; i++)
        //{
        //    Gizmos.DrawWireSphere(PointPositions[i], 1f);
        //    Handles.Label(PointPositions[i], floats[i].ToString(), style);
        //}

   // }
}
