using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponSystem))]
public class WeaponSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty tipoArma =
            serializedObject.FindProperty("tipoArma");

        EditorGUILayout.PropertyField(tipoArma);

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Configuración General",
            EditorStyles.boldLabel
        );

        Campo("danio");
        Campo("tiempoEntreAtaques");

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Audio",
            EditorStyles.boldLabel
        );

        Campo("sonidoDisparo");
        Campo("sonidoRecarga");
        Campo("sonidoSinMunicion");
        Campo("sonidoMelee");

        GUILayout.Space(10);

        WeaponType tipo =
            (WeaponType)tipoArma.enumValueIndex;

        switch (tipo)
        {
            case WeaponType.Melee:

                EditorGUILayout.LabelField(
                    "Configuración Melee",
                    EditorStyles.boldLabel
                );

                Campo("meleeTrigger");
                Campo("animator");
                Campo("tiempoHitbox");

                break;

            case WeaponType.Firearm:

                EditorGUILayout.LabelField(
                    "Configuración Arma de Fuego",
                    EditorStyles.boldLabel
                );

                Campo("armaAutomatica");

                GUILayout.Space(5);

                EditorGUILayout.LabelField(
                    "Munición",
                    EditorStyles.boldLabel
                );

                Campo("usarMunicion");
                Campo("tipoMunicion");
                Campo("municionEnCargador");
                Campo("tamanoCargador");
                Campo("municionReserva");
                Campo("reservaMaxima");
                Campo("multiplicadorMejoraReserva");
                Campo("tiempoRecarga");

                GUILayout.Space(5);

                EditorGUILayout.LabelField(
                    "Configuración Bala",
                    EditorStyles.boldLabel
                );

                Campo("idPoolBala");
                Campo("puntoDisparo");
                Campo("velocidadBala");
                Campo("tiempoVidaBala");
                Campo("balasPorDisparo");
                Campo("dispersion");
                Campo("atravesarEnemigos");
                Campo("efectoDisparo");

                break;
        }

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Debug",
            EditorStyles.boldLabel
        );

        Campo("mostrarLogs");

        serializedObject.ApplyModifiedProperties();
    }

    private void Campo(string nombrePropiedad)
    {
        SerializedProperty propiedad =
            serializedObject.FindProperty(nombrePropiedad);

        if (propiedad != null)
            EditorGUILayout.PropertyField(propiedad);
    }
}