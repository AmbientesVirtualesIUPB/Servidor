using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ObjetoPersonalizable))]
public class ObjetoPersonalizableDrawer : PropertyDrawer
{
    private static Material previewMaterial;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Definir dimensiones
        float previewSize = 55; // Tamaño de las vistas previas
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float previewAreaWidth = previewSize + spacing;

        // Rectángulos para las vistas previas
        Rect spritePreviewRect = new Rect(position.x, position.y, previewSize, previewSize);
        Rect texturePreviewRect = new Rect(position.x, position.y + previewSize + spacing, previewSize, previewSize);

        // Rectángulo para los campos normales
        Rect fieldsRect = new Rect(position.x + previewAreaWidth, position.y, position.width - previewAreaWidth, position.height);

        // Dibujar la vista previa del Sprite
        SerializedProperty spriteProp = property.FindPropertyRelative("sprite");
        Sprite sprite = spriteProp.objectReferenceValue as Sprite;
        if (sprite != null)
        {
            DrawSpritePreview(spritePreviewRect, sprite);
        }
        else
        {
            EditorGUI.HelpBox(spritePreviewRect, "No hay Sprite", MessageType.Info);
        }

        // Dibujar la vista previa de la textura txRGB
        SerializedProperty txRGBProp = property.FindPropertyRelative("txRGB");
        Texture2D txRGB = txRGBProp.objectReferenceValue as Texture2D;
        if (txRGB != null)
        {
            EditorGUI.DrawPreviewTexture(texturePreviewRect, txRGB);
        }
        else
        {
            EditorGUI.HelpBox(texturePreviewRect, "No hay textura RGB", MessageType.Info);
        }

        // Dibujar los campos normales
        EditorGUI.PropertyField(fieldsRect, property, true);

        EditorGUI.EndProperty();
    }

    private void DrawSpritePreview(Rect rect, Sprite sprite)
    {
        if (previewMaterial == null)
        {
            // Crear un material con un shader transparente
            Shader shader = Shader.Find("Unlit/Transparent");
            previewMaterial = new Material(shader);
        }

        // Dibujar el Sprite con soporte de transparencia
        Texture2D texture = sprite.texture;
        previewMaterial.mainTexture = texture;

        // Calcular los recortes correctos
        Rect textureRect = sprite.textureRect;
        Rect uvRect = new Rect(
            textureRect.x / texture.width,
            textureRect.y / texture.height,
            textureRect.width / texture.width,
            textureRect.height / texture.height
        );

        Graphics.DrawTexture(rect, texture, uvRect, 0, 0, 0, 0, previewMaterial);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float previewSize = 70; // Altura del cuadro de vistas previas
        float defaultHeight = EditorGUI.GetPropertyHeight(property, label, true); // Altura de los campos normales
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        return Mathf.Max(previewSize * 2 + spacing, defaultHeight); // Altura total basada en el área más grande
    }
}
