using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

namespace RedScripts
{
    public class DungeonController : MonoBehaviour
    {
        public enum LocomotionMode
        {
            Teleporter,
            ArmSwinger
        }

        private DungeonTeleportArea[] teleportAreas;

        [Header("Locomotion settings")]
        [SerializeField]
        private Teleport teleportController;
        [SerializeField]
        private ArmSwinger armSwingerController;
        [SerializeField]
        public GameObject teleportAreaPrefab;

 

        
        [SerializeField]
        public LocomotionMode locomotionMode;     

        void Start()
        {
            
            teleportAreas = FindObjectsOfType<DungeonTeleportArea>();

            if (GetArg("-locomotion") != null)
            {
                var locomotionArg = GetArg("-locomotion");
                if (locomotionArg == "armswinger")
                {
                    locomotionMode = LocomotionMode.ArmSwinger;
                }
                else if(locomotionArg == "teleport")
                {
                    locomotionMode = LocomotionMode.Teleporter;
                }            
            }         

                SwitchLocomotionMode();

        }

        private void SwitchLocomotionMode()
        {
            switch (locomotionMode)
            {
                case LocomotionMode.Teleporter:
                    teleportController.canTeleport = true;
                    armSwingerController.enabled = false;
                    armSwingerController.armSwingNavigation = false;
                    break;
                case LocomotionMode.ArmSwinger:
                    teleportController.canTeleport = false;
                    armSwingerController.enabled = true;
                    armSwingerController.armSwingNavigation = true;
                    break;
                default:
                    break;
            }
        }

        private void OnValidate()
        {
            if (teleportController != null && armSwingerController != null)
            {
                SwitchLocomotionMode();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Press space to change locomotion mode
            {
                if (locomotionMode == LocomotionMode.Teleporter)
                {
                    locomotionMode = LocomotionMode.ArmSwinger;
                }
                else
                {
                    locomotionMode = LocomotionMode.Teleporter;
                }
            }
        }

        private static string GetArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }


    }


    [CustomEditor(typeof(DungeonController))]
    public class DungeonControlleraEditor : Editor
    {
        private GameObject[] dungeonFloors;
        private GameObject[] dungeonStairs;
        private GameObject[] dungeonBride;
        private GameObject teleportAreaPrefab;

        private void OnEnable()
        {
            Debug.Log("EDITOR MODE");

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DungeonController dungeonControllerScript = (DungeonController)target;
            if (Selection.activeGameObject != dungeonControllerScript.gameObject) return;

            var guiStyle = new GUIStyle();
            guiStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("\nEditor Helper", guiStyle);

            GUILayout.Label("Dungeon Teleport Area Prefab");

            teleportAreaPrefab = dungeonControllerScript.teleportAreaPrefab;  //= EditorGUILayout.ObjectField(teleportAreaPrefab, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Select Teleport Areas"))
            {
                dungeonFloors = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "dungeon_floor" && x.activeInHierarchy).ToArray();
                dungeonStairs = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "stairs" && x.activeInHierarchy).ToArray();
                dungeonBride = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name.Contains("wood bridge") && x.activeInHierarchy).ToArray();

                Selection.objects = dungeonFloors.Concat(dungeonStairs).ToArray();
            }
            if (GUILayout.Button("Update Teleport Areas"))
            {
                dungeonFloors = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "dungeon_floor" && x.activeInHierarchy).ToArray();
                dungeonStairs = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "stairs" && x.activeInHierarchy).ToArray();
                dungeonBride = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name.Contains("wood bridge") && x.activeInHierarchy).ToArray();


                foreach (var floor in dungeonFloors)
                {
                    if (floor.GetComponentInChildren<DungeonTeleportArea>() == null)
                    {
                        var tempArea = PrefabUtility.InstantiatePrefab(teleportAreaPrefab) as GameObject;
                        tempArea.transform.parent = floor.transform;
                        tempArea.transform.position = Vector3.zero;
                        tempArea.transform.localPosition = Vector3.zero;
                        tempArea.transform.localRotation = dungeonControllerScript.transform.rotation;

                        DestroyImmediate(floor.GetComponent<BoxCollider>());
                        tempArea.GetComponent<DungeonTeleportArea>().showMarkers = true;
                  
                    }
                }

                foreach (var stair in dungeonStairs)
                {
                    if (stair.GetComponentInChildren<DungeonTeleportArea>() == null)
                    {
                        var tempArea = PrefabUtility.InstantiatePrefab(teleportAreaPrefab) as GameObject;
                        tempArea.transform.parent = stair.transform;
                        tempArea.transform.localRotation = Quaternion.Euler(0f,-26f,0f);// dungeonControllerScript.transform.rotation;
                        tempArea.transform.localPosition = new Vector3(2.23f,0f,1.44f);// dungeonControllerScript.transform.localPosition;
                        //tempArea.transform.position = dungeonControllerScript.transform.position;

                       // DestroyImmediate(floor.GetComponent<BoxCollider>());
                        tempArea.GetComponent<DungeonTeleportArea>().showMarkers = true;

                    }
                }

                foreach (var bridge in dungeonBride)
                {
                    if (bridge.GetComponentInChildren<DungeonTeleportArea>() == null)
                    {
                        var tempArea = PrefabUtility.InstantiatePrefab(teleportAreaPrefab) as GameObject;
                        tempArea.transform.parent = bridge.transform;
                        tempArea.transform.position = Vector3.zero;
                        tempArea.transform.localPosition = Vector3.zero;
                        tempArea.transform.localRotation = Quaternion.Euler(90,0,0);
                        tempArea.transform.localScale = new Vector3(0.3876143f, 1f, 1f);
                        



                        tempArea.GetComponent<DungeonTeleportArea>().showMarkers = true;

                    }
                }



            }
            if (GUILayout.Button("Remove Teleport Areas"))
            {
                dungeonFloors = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "dungeon_floor" && x.activeInHierarchy).ToArray();
                dungeonStairs = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "stairs" && x.activeInHierarchy).ToArray();
                dungeonBride = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name.Contains("wood bridge") && x.activeInHierarchy).ToArray();


                foreach (var floor in dungeonFloors.Concat(dungeonStairs).Concat(dungeonBride))
                {
                    if (floor.GetComponentInChildren<DungeonTeleportArea>() != null)
                    {
                        if (Application.isEditor)
                            DestroyImmediate(floor.GetComponentInChildren<DungeonTeleportArea>().gameObject);

                        else
                            Destroy(floor.GetComponentInChildren<DungeonTeleportArea>().gameObject);
                    }
                }

            }
            GUILayout.Label("Teleport Area Settings", guiStyle);
            if (GUILayout.Button("Enable Teleport Visuals"))
            {
                var teleportAreas = Resources.FindObjectsOfTypeAll<DungeonTeleportArea>().Where(x => x.gameObject.activeInHierarchy).ToList();
                teleportAreas.ForEach(t => t.showMarkers = true);

            }

            if (GUILayout.Button("Disable Teleport Visuals"))
            {
                var teleportAreas = Resources.FindObjectsOfTypeAll<DungeonTeleportArea>().Where(x => x.gameObject.activeInHierarchy).ToList();
                teleportAreas.ForEach(t => t.showMarkers = false);
            }
           
        }


    }
}
