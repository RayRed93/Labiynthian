
using UnityEngine;
using Valve.VR.InteractionSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;

namespace RedScripts
{
	public class DungeonTeleportArea : TeleportMarkerBase
	{
		public Bounds meshBounds { get; private set; }

		private MeshRenderer meshRanderer;
		private int tintColorId = 0;
		private Color visibleTintColor = Color.clear;
		private Color highlightedTintColor = Color.clear;
		private Color lockedTintColor = Color.clear;
		private bool highlighted = false;

        [SerializeField]
        public bool showMarkers = false;

        private MeshFilter meshShape;

        private GameObject teleportAreaContainer;

		public void Awake()
		{
            meshShape = this.transform.parent.GetComponent<MeshFilter>();

           


            var meshFilter = this.GetComponent<MeshFilter>();
            //meshFilter.mesh = meshShape.mesh;
            //meshFilter.sharedMesh = meshShape.sharedMesh;
            
            meshRanderer = this.GetComponent<MeshRenderer>();
            meshRanderer.material = Teleport.instance.areaVisibleMaterial;
            meshRanderer.sharedMaterial = Teleport.instance.areaVisibleMaterial;

			tintColorId = Shader.PropertyToID( "_TintColor" );

            CalculateBounds();
		}


		//-------------------------------------------------
		public void Start()
		{
			visibleTintColor = Teleport.instance.areaVisibleMaterial.GetColor( tintColorId );
			highlightedTintColor = Teleport.instance.areaHighlightedMaterial.GetColor( tintColorId );
			lockedTintColor = Teleport.instance.areaLockedMaterial.GetColor( tintColorId );

			
		}


		//-------------------------------------------------
		public override bool ShouldActivate( Vector3 playerPosition )
		{
            if (Vector3.Distance(playerPosition, this.transform.position) < 15)
            {
                return true;
            }
            else
            {
                return false;
            }
		}


		//-------------------------------------------------
		public override bool ShouldMovePlayer()
		{
			return true;
		}


		//-------------------------------------------------
		public override void Highlight( bool highlight )
		{
			if ( !locked )
			{
				highlighted = highlight;

                if (showMarkers)
                {
                    meshRanderer.enabled = true;
                    if (highlight)
                    {
                        meshRanderer.material = Teleport.instance.areaHighlightedMaterial;
                    }
                    else
                    {
                        meshRanderer.material = Teleport.instance.areaVisibleMaterial;
                    } 
                }
                else
                {
                    meshRanderer.enabled = false;
                }
			}
		}


		//-------------------------------------------------
		public override void SetAlpha( float tintAlpha, float alphaPercent )
		{
            if (showMarkers)
            {
                Color tintedColor = GetTintColor();
                tintedColor.a *= alphaPercent;
                meshRanderer.material.SetColor(tintColorId, tintedColor); 
            }
		}


		//-------------------------------------------------
		public override void UpdateVisuals()
		{
            if (showMarkers)
            {
                meshRanderer.enabled = true;
                if (locked)
                {
                    meshRanderer.material = Teleport.instance.areaLockedMaterial;
                }
                else
                {
                    meshRanderer.material = Teleport.instance.areaVisibleMaterial;
                } 
            }
            else
            {
                meshRanderer.enabled = false;
            }
		}


		//-------------------------------------------------
		public void UpdateVisualsInEditor()
		{
            meshRanderer = GetComponent<MeshRenderer>();

            if (showMarkers)
            {
                meshRanderer.enabled = true;
                if (locked)
                {
                    meshRanderer.sharedMaterial = Teleport.instance.areaLockedMaterial;
                }
                else
                {
                    meshRanderer.sharedMaterial = Teleport.instance.areaVisibleMaterial;
                } 
            }
            else
            {
                meshRanderer.enabled = false;
            }
		}


		//-------------------------------------------------
		private bool CalculateBounds()
		{
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if ( meshFilter == null )
			{
				return false;
			}

			Mesh mesh = meshFilter.sharedMesh;
			if ( mesh == null )
			{
				return false;
			}

			meshBounds = mesh.bounds;
			return true;
		}


		//-------------------------------------------------
		private Color GetTintColor()
		{
			if ( locked )
			{
				return lockedTintColor;
			}
			else
			{
				if ( highlighted )
				{
					return highlightedTintColor;
				}
				else
				{
					return visibleTintColor;
				}
			}
		}
	}


#if UNITY_EDITOR
	//-------------------------------------------------------------------------
	[CustomEditor( typeof(DungeonTeleportArea) )]
	public class TeleportAreaEditor : Editor
	{
		//-------------------------------------------------
		void OnEnable()
		{
			if ( Selection.activeTransform != null )
			{
                DungeonTeleportArea teleportArea = Selection.activeTransform.GetComponent<DungeonTeleportArea>();
				if ( teleportArea != null )
				{
					teleportArea.UpdateVisualsInEditor();
				}
			}
		}


		//-------------------------------------------------
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if ( Selection.activeTransform != null )
			{
                DungeonTeleportArea teleportArea = Selection.activeTransform.GetComponent<DungeonTeleportArea>();
				if ( GUI.changed && teleportArea != null )
				{
					teleportArea.UpdateVisualsInEditor();
				}
			}
		}
	}
#endif
}
