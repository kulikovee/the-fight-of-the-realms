using UnityEngine;

public class CameraController : MonoBehaviour
{

    PlayerController[] players;
    LevelController level;
    Vector3 targetPosition = Vector3.zero;
    Vector3 arenaPosition = new Vector3(0, 12, -7.3f);
    Vector3 arenaRotation = new Vector3(60, 0, 0);
    Vector3 platformerPosition = new Vector3(0, 7, -7.3f);
    Vector3 platformerRotation = new Vector3(35, 0, 0);
    Vector3 averagePosition = Vector3.zero;

    void Start()
    {
        level = LevelController.GetLevel();
        players = GameObject.FindObjectsOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (level.IsPlatformer())
        {
            var alivePlayersCount = 0;
            var maxPlayersDistance = 0f;
            var previousPosition = averagePosition;
            averagePosition = Vector3.zero;

            foreach (var player in players)
            {
                foreach(var _player in players)
                {
                    var distance = Vector3.Distance(player.transform.position, _player.transform.position);
                    if (distance > maxPlayersDistance)
                    {
                        maxPlayersDistance = distance;
                    }
                }

                if (player.GetUnit().IsAlive())
                {
                    alivePlayersCount++;
                    averagePosition += player.transform.position;
                }
            }

            if (alivePlayersCount > 0)
            {
                targetPosition = platformerPosition + Vector3.right * averagePosition.x / alivePlayersCount;

                var deltaPosition = (transform.position - targetPosition) - Vector3.right * 2;
                transform.position -= deltaPosition / 40f;
                transform.rotation = Quaternion.Euler(platformerRotation.x, platformerRotation.y, platformerRotation.z);
            }
        } else
        {
            transform.position = arenaPosition;
            transform.rotation = Quaternion.Euler(arenaRotation.x, arenaRotation.y, arenaRotation.z);
        }
    }
}
