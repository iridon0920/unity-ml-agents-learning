using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RaycastAgent : Agent
{
     Rigidbody _rBody;
     int lastCheckPoint;
     int checkPointCount;

    public override void Initialize()
    {
        this._rBody = GetComponent<Rigidbody>();
    }

    // エピソード開始時に呼ばれる
    public override void OnEpisodeBegin()
    {
        // Agent落下時
        if (this.transform.localPosition.y < 0)
        {
            this._rBody.angularVelocity = Vector3.zero;
            this._rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(3.5f, 0.5f, 0.0f);
        }

        // 周回数リセット
        lastCheckPoint = 0;
        checkPointCount = 0;
    }

    // 観察時に実行
    public override void CollectObservations(VectorSensor sensor)
    {
        // エージェントX速度
        sensor.AddObservation(_rBody.velocity.x);
        // エージェントY速度
        sensor.AddObservation(_rBody.velocity.z);
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> vectorAction = actions.DiscreteActions;

        // 行動（vectorAction）の内容に応じて、rBody経由でRollerAgentへ力を加える
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int action = vectorAction[0];
        if (action == 1) dirToGo = transform.forward;
        if (action == 2) dirToGo = transform.forward * -1.0f;
        if (action == 3) rotateDir = transform.up * -1.0f;
        if (action == 4) rotateDir = transform.up;

        this.transform.Rotate(rotateDir, Time.deltaTime * 200f);

        Vector3 targetPosition = _rBody.position +  dirToGo * 0.1f;
        _rBody.position = targetPosition;

       // ステップごとの報酬
       AddReward(-0.001f);
    }

    // チェックポイント衝突時に実行
    public void EnterCheckPoint(int checkPoint)
    {
        // 次のチェックポイントに衝突
        if (checkPoint == (lastCheckPoint + 1) % 4)
        {
            this.checkPointCount++;

            // ゴール
            if (checkPointCount >= 4)
            {
                AddReward(2.0f);
                EndEpisode();
            }
        }
        // 前のチェックポイントに衝突
        else if(checkPoint == (lastCheckPoint -1 + 4)% 4)
        {
            checkPointCount--;
        }

        // 最終チェックポイント更新
        lastCheckPoint = checkPoint;
    }

    // ヒューリスティックモードの行動時に実行
    public override void Heuristic(in ActionBuffers actionBuffers)
    {
        ActionSegment<int> actionsOut = actionBuffers.DiscreteActions;

        actionsOut[0] = 0;
        if (Input.GetKey(KeyCode.UpArrow)) actionsOut[0] = 1;
        if (Input.GetKey(KeyCode.DownArrow)) actionsOut[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow)) actionsOut[0] = 3;
        if (Input.GetKey(KeyCode.RightArrow)) actionsOut[0] = 4;
    }
}
