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

        EditorGUILayout.LabelField(
            "CONFIGURACIÓN GENERAL",
            EditorStyles.boldLabel
        );

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

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "tiempoHitbox"
                    )
                );

                break;

            case WeaponType.Firearm:

                EditorGUILayout.LabelField(
                    "CONFIGURACIÓN FIREARM",
                    EditorStyles.boldLabel
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "armaAutomatica"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "usarMunicion"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "municionActual"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "municionMaxima"
                    )
                );

                GUILayout.Space(5);

                EditorGUILayout.LabelField(
                    "CONFIGURACIÓN BALA",
                    EditorStyles.boldLabel
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "idPoolBala"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "puntoDisparo"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "velocidadBala"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "tiempoVidaBala"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "balasPorDisparo"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "dispersion"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "atravesarEnemigos"
                    )
                );

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        "efectoDisparo"
                    )
                );

                break;
        }

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "DEBUG",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(
                "mostrarLogs"
            )
        );

        serializedObject.ApplyModifiedProperties();
    }
}