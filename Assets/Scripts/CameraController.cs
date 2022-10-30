using UnityEngine;

public class CameraController : MonoBehaviour
{

    PlayerController[] players;
    LevelController level;
    Camera camera;
    Vector3 defaultPosition = new Vector3(0, 12, -7.3f);
    Vector3 defaultRotation = new Vector3(60, 0, 0);
    float defaultFieldOfView = 55;

    void Start()
    {
        camera = GetComponent<Camera>();
        level = LevelController.GetLevel();
        players = GameObject.FindObjectsOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (level.IsPlatformer())
        {
            var averagePosition = Vector3.zero;
            var alivePlayersCount = 0;
            var maxPlayersDistance = 0f;

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
                var targetPosition = averagePosition / alivePlayersCount + new Vector3(0, 7, -7.3f);
                transform.position -= (transform.position - targetPosition) / 10f;
                transform.rotation = Quaternion.Euler(35, 0, 0);
                camera.fieldOfView = Mathf.Min(80, defaultFieldOfView + maxPlayersDistance * 1.5f);
            }
        } else
        {
            transform.position = defaultPosition;
            transform.rotation = Quaternion.Euler(defaultRotation.x, defaultRotation.y, defaultRotation.z);
            camera.fieldOfView = defaultFieldOfView;
        }
    }
}
