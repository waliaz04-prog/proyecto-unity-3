using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponSystem))]
public class WeaponSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty tipoArma =
            serializedObject.FindProperty(
                "tipoArma"
            );

        EditorGUILayout.PropertyField(
            tipoArma
        );

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(
                "danio"
            )
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(
                "tiempoEntreAtaques"
            )
        );

        WeaponType tipo =
            (WeaponType)
            tipoArma.enumValueIndex;

        GUILayout.Space(10);

        switch (tipo)
        {
            // MELEE
            case WeaponType.Melee:

                EditorGUILayout.LabelField(
                    "CONFIGURACIÓN MELEE",
                    EditorStyles.boldLabel
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "meleeTrigger"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "animator"
                    )
                );

                break;

            // FIREARM
            case WeaponType.Firearm:

                EditorGUILayout.LabelField(
                    "CONFIGURACIÓN FIREARM",
                    EditorStyles.boldLabel
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "camaraJugador"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "capaEnemigo"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "rangoDisparo"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "armaAutomatica"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "municion"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "municionMaxima"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "efectoDisparo"
                    )
                );

                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}