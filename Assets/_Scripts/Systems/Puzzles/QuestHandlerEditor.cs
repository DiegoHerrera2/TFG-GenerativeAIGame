using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(QuestHandlerBase), true)]
    public class QuestHandlerEditor : Editor
    {
        // Make a field to put a string separated with commas and a button to add the tags to the list
        private string _tagString = "";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var questHandler = (QuestHandlerBase) target;
            GUILayout.Space(10);
            GUILayout.Label("Quest Tags");
            _tagString = EditorGUILayout.TextField("Tags", _tagString);
            if (GUILayout.Button("Add Tags"))
            {
                var tags = _tagString.Split(',');
                var tagList = tags.Select(tag => tag.Trim()).ToList();
                questHandler.SetQuestTags(tagList);
            }
        }
    }
    #endif
}