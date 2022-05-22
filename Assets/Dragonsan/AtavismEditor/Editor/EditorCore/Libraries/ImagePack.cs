using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    // Static Class to handle all UI layout information 
    public static class ImagePack
    {
        // GUI Style
        public static string skin = "GUISkin";
        public static GUISkin atavismSkin = (GUISkin)Resources.Load(skin, typeof(GUISkin));

        // Main Window Layout Dimensions 
        public static int layoutWidth = 910;
        public static int layoutHeight = 632;
        public static int layoutTop = 54;
        public static int layoutTopMargin = 65;
        public static int layoutLeftMargin = 28;

        // Categories Layout
        public static int layoutCategoryHeight = 575;
        public static int layoutCategoryWidth = 85;
        public static int layoutCategoryMargin = 120;
        public static int layoutSpace = 5;

        // Functions Layout
        public static int layoutFunctionTop = 134;
        public static int layoutFunctionTopMargin = 140;
        public static int layoutFunctionMargin = 145;
        public static int layoutFunctionRightMargin = 379;
        public static int layoutFunctionWidth = 224;
        public static int layoutFunctionHeight = 490;

        // Inspector Layout
        public static int layoutInspectorTopMargin = 100;
        public static int layoutInspectorMargin = 420;
        public static int layoutInspectorBoxWidth = 470;
        public static int layoutInspectorBoxHeight = 500;

        // Main background textures
        // Main Window Background
       // public static string plugin_bg = "AT_pluginpage_bg";
       // public static Texture mainWindow = (Texture)Resources.Load(plugin_bg, typeof(Texture));
        public static string plugin_bg = "AT_bg";
        public static Texture mainWindow = (Texture)Resources.Load(plugin_bg, typeof(Texture));
        public static string plugin_logo = "AT_pluginpage_bg_logo";
        public static Texture mainWindow_logo = (Texture)Resources.Load(plugin_logo, typeof(Texture));
        public static string plugin_topbar = "AT_pluginpage_bg_topbar";
        public static Texture mainWindow_topbar = (Texture)Resources.Load(plugin_topbar, typeof(Texture));
        public static string plugin_category = "AT_pluginpage_bg_category";
        public static Texture mainWindow_category_top = (Texture)Resources.Load(plugin_category+"_top", typeof(Texture));
        public static Texture mainWindow_category_mid = (Texture)Resources.Load(plugin_category+"_mid", typeof(Texture));
        public static Texture mainWindow_category_bottom = (Texture)Resources.Load(plugin_category+"_bottom", typeof(Texture));
        public static string plugin_plugin = "AT_pluginpage_bg_plugin";
        public static Texture mainWindow_plugin_top = (Texture)Resources.Load(plugin_plugin+"_top", typeof(Texture));
        public static Texture mainWindow_plugin_mid = (Texture)Resources.Load(plugin_plugin+"_mid", typeof(Texture));
        public static Texture mainWindow_plugin_bottom = (Texture)Resources.Load(plugin_plugin+"_bottom", typeof(Texture));

        public static string plugin_inspec = "AT_pluginpage_bg_w";
        public static Texture mainWindow_inspec_top = (Texture)Resources.Load(plugin_inspec + "_top", typeof(Texture));
        public static Texture mainWindow_inspec_top_l = (Texture)Resources.Load(plugin_inspec + "_top_left", typeof(Texture));
        public static Texture mainWindow_inspec_top_r = (Texture)Resources.Load(plugin_inspec + "_top_right", typeof(Texture));
        public static Texture mainWindow_inspec_mid = (Texture)Resources.Load(plugin_inspec + "_mid", typeof(Texture));
        public static Texture mainWindow_inspec_mid_l = (Texture)Resources.Load(plugin_inspec + "_mid_left", typeof(Texture));
        public static Texture mainWindow_inspec_mid_r = (Texture)Resources.Load(plugin_inspec + "_mid_right", typeof(Texture));
        public static Texture mainWindow_inspec_bottom = (Texture)Resources.Load(plugin_inspec + "_bottom", typeof(Texture));
        public static Texture mainWindow_inspec_bottom_li = (Texture)Resources.Load(plugin_inspec + "_bottom_line", typeof(Texture));
        public static Texture mainWindow_inspec_bottom_l = (Texture)Resources.Load(plugin_inspec + "_bottom_left", typeof(Texture));
        public static Texture mainWindow_inspec_bottom_r = (Texture)Resources.Load(plugin_inspec + "_bottom_right", typeof(Texture));



        // Plugin Editor Window Background
        public static string pluginEditor_bg = "AT_plugin_editor_page_bg";
        public static Texture pluginEditorWindow = (Texture)Resources.Load(pluginEditor_bg, typeof(Texture));
        // Login Window Background
        public static string login_bg = "AT_loginpage_bg";
        public static Texture loginWindow = (Texture)Resources.Load(login_bg, typeof(Texture));
        // Login Box
        public static int layoutLoginTopMargin = 150;
        public static int layoutLoginLeftMargin = 289; //350;
        public static string login_box = "AT_login_bg";
        public static Texture loginBox = (Texture)Resources.Load(login_box, typeof(Texture));

        // Scroll bar textures
        public static string scrollTrack = "AT_scrollebar_bg";
        public static Texture scrollBar = (Texture)Resources.Load(scrollTrack, typeof(Texture));
        public static Texture scrollCategory = (Texture)Resources.Load(scrollTrack, typeof(Texture));
        public static Texture scrollFunction = (Texture)Resources.Load(scrollTrack, typeof(Texture));
        public static string scrollButton = "AT_button_scrollebar_slider";
        public static Texture scrollSlider = (Texture)Resources.Load(scrollButton, typeof(Texture));
        public static string scrollArrows = "AT_button_scrollebar_arrow";
        public static Texture scrollArrowUp = (Texture)Resources.Load(scrollArrows + "Up", typeof(Texture));
        public static Texture scrollArrowUpOver = (Texture)Resources.Load(scrollArrows + "Up_over", typeof(Texture));
        public static Texture scrollArrowDown = (Texture)Resources.Load(scrollArrows + "Down", typeof(Texture));
        public static Texture scrollArrowDownOver = (Texture)Resources.Load(scrollArrows + "Down_over", typeof(Texture));

        // Atavism Logo textures
        public static string icon_AT = "AT_icon_atavismLogo";
        public static Texture atLogo = (Texture)Resources.Load(icon_AT, typeof(Texture));

        // Lock icons textures
        public static string icon_lock = "AT_icon_lock";
        public static Texture lockGreen = (Texture)Resources.Load(icon_lock + "_green", typeof(Texture));
        public static Texture lockGrey = (Texture)Resources.Load(icon_lock + "_grey", typeof(Texture));
        public static Texture LockRed = (Texture)Resources.Load(icon_lock + "_red", typeof(Texture));

        // All button icons textures
        public static string all_bt = "AT_button_all";
        public static Texture allButton = (Texture)Resources.Load(all_bt, typeof(Texture));
        public static Texture allButtonOver = (Texture)Resources.Load(all_bt + "_over", typeof(Texture));
        public static Texture allButtonSelected = (Texture)Resources.Load(all_bt + "_selected", typeof(Texture));

        // Tab textures
        public static string tabs_icons = "AT_tab";
        public static string tab_over = "_over";
        public static Texture tabCreate = (Texture)Resources.Load(tabs_icons + "_create", typeof(Texture));
        public static Texture tabCreateOver = (Texture)Resources.Load(tabs_icons + "_create" + "_over", typeof(Texture));
        public static Texture tabEdit = (Texture)Resources.Load(tabs_icons + "_edit", typeof(Texture));
        public static Texture tabEditOver = (Texture)Resources.Load(tabs_icons + "_edit" + "_over", typeof(Texture));
        public static Texture tabDoc = (Texture)Resources.Load(tabs_icons + "_doc", typeof(Texture));
        public static Texture tabDocOver = (Texture)Resources.Load(tabs_icons + "_doc" + "_over", typeof(Texture));

        public static Texture tabRestore = (Texture)Resources.Load(tabs_icons + "_rec", typeof(Texture));
        public static Texture tabRestoreOver = (Texture)Resources.Load(tabs_icons + "_rec" + "_over", typeof(Texture));

        public static float tabSpace = 1;
        public static float tabMargin = 55;
        public static float tabTop = -34;
        public static float tabLeft = 310;

        // Functions button textures
        public static string button_bg = "AT_button_plugin";
        public static Texture button = (Texture)Resources.Load(button_bg, typeof(Texture));
        public static Texture buttonOver = (Texture)Resources.Load(button_bg + "_over", typeof(Texture));

        // Functions search textures
        public static string search_bt = "AT_icon_search";
        public static Texture buttonSearch = (Texture)Resources.Load(search_bt, typeof(Texture));
        public static Texture buttonSearchOver = (Texture)Resources.Load(search_bt + "_over", typeof(Texture));

        public static string icon_select = "AT_icon_background";
        public static Texture iconSelect = (Texture)Resources.Load(icon_select, typeof(Texture));

        // Header layout
        public static int headerFontSize = 18;
        public static Color headerFontColor = Color.white;

        // Field layout
        // Static Label 
        public static int fieldFontSize = 12;
        public static Color fieldFontColor = Color.grey;
        public static float lineSpace = 8;
        public static float fieldHeight = 28;
        public static float fieldMargin = 16;
        // Text Field
        public static int textFieldFontSize = 12;
        public static Color textFieldFontColor = new Color(180, 180, 180);
        public static int textFieldPadding = 6;
        // Background	
        public static string inputField = "AT_input_field";
        public static Texture fieldBackground = (Texture)Resources.Load(inputField, typeof(Texture));
        // Filter Background	
        public static string filterField = "AT_filter_field";
        public static Texture filterBackground = (Texture)Resources.Load(filterField, typeof(Texture));


        // Button layout
        public static int buttonFontSize = 11;
        public static Color buttonFontColor = Color.white;
        public static float buttonMargin = 20;
        public static string buttonRegular = "AT_button";
        public static Texture buttonBackground = (Texture)Resources.Load(buttonRegular, typeof(Texture));
        public static Texture buttonBackgroundOver = (Texture)Resources.Load(buttonRegular + "_over", typeof(Texture));

        // Label layout
        public static float labelWidth = 200;
        public static float labelHeight = 20;
        public static float labelMargin = 0;
        public static int labelFontSize = 16;
        public static Color labelFontColor = Color.white;

        // Text layout
        public static int textFontSize = 12;
        public static Color textFontColor = Color.white;

        // Checkbox layout
        public static string checkboxRegular = "AT_checkbox";
        public static Texture checkboxBackground = (Texture)Resources.Load(checkboxRegular, typeof(Texture));
        public static Texture checkboxBackgroundSelected = (Texture)Resources.Load(checkboxRegular + "_active", typeof(Texture));

        // Combobox layout
        public static float comboboxWidth = 200;
        public static float comboboxHeight = 20;
        public static float comboboxMargin = 40;
        public static string comboboxRegular = "AT_dropdown_displaybox";
        public static Texture comboboxBackground = (Texture)Resources.Load(comboboxRegular, typeof(Texture));
        public static Texture comboboxBackgroundSelected = (Texture)Resources.Load(comboboxRegular + "_overItem", typeof(Texture));

        // Selector layout
        public static string selectorRegular = "AT_button_dropdown";
        public static Texture selectorBackground = (Texture)Resources.Load(selectorRegular, typeof(Texture));
        public static string selectorLarge = "AT_large_button_dropdown";
        public static Texture selectorLargeBackground = (Texture)Resources.Load(selectorLarge, typeof(Texture));

        //Status 
        public static string statusIcon = "AT_radiobutton";
        public static Texture status = (Texture)Resources.Load(statusIcon, typeof(Texture));
        public static Texture statusBlue = (Texture)Resources.Load(statusIcon + "_blue", typeof(Texture));
        public static Texture statusRed = (Texture)Resources.Load(statusIcon + "_red", typeof(Texture));


        // Components Layout
        public static float innerMargin = 10;

        /*
         * Methods to create GUI Styles 
         * 
         */

        // Style for Headers
        public static GUIStyle HeaderStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = headerFontSize;
            style.normal.textColor = headerFontColor;
            return style;
        }

        // Style for Fields
        public static GUIStyle LabelStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = labelFontSize;
            style.normal.textColor = labelFontColor;
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }

        // Style for Fields
        public static GUIStyle TextStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = textFontSize;
            style.normal.textColor = textFontColor;
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }

        // Style for Fields
        public static GUIStyle TextMultiLineStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = textFontSize;
            style.normal.textColor = textFontColor;
            style.alignment = TextAnchor.MiddleLeft;
            style.richText = true;
            style.wordWrap = true;
            return style;
        }

        // Style for Fields
        public static GUIStyle FieldStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fieldFontSize;
            style.normal.textColor = fieldFontColor;
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }

        public static GUIStyle IconStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fieldFontSize;
            style.normal.textColor = fieldFontColor;
            style.normal.background = (Texture2D)iconSelect;
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }

        // Style for TextFields
        public static GUIStyle TextAreaStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = textFieldFontSize;
            style.normal.textColor = textFieldFontColor;
            style.padding.left = textFieldPadding;
            style.alignment = TextAnchor.UpperLeft;
            style.wordWrap = true;
            style.stretchHeight = true;
            style.normal.background = (Texture2D)fieldBackground;
            return style;
        }

        // Style for TextFields
        public static GUIStyle TextFieldStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = textFieldFontSize;
            style.normal.textColor = textFieldFontColor;
            style.padding.left = textFieldPadding;
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.background = (Texture2D)fieldBackground;
            return style;
        }
        public static GUIStyle TextFieldBoldStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = textFieldFontSize+3;
            style.normal.textColor = textFieldFontColor;
            style.padding.left = textFieldPadding;
            style.alignment = TextAnchor.MiddleLeft;
            style.fontStyle = FontStyle.Bold;
           // style.normal.background = (Texture2D)fieldBackground;
            return style;
        }

        // Style for TextFields
        public static GUIStyle FilterFieldStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = textFieldFontSize;
            style.normal.textColor = textFieldFontColor;
            style.padding.left = textFieldPadding;
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.background = (Texture2D)filterBackground;
            return style;
        }

        // Style for Buttons
        public static GUIStyle ButtonStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = buttonFontSize;
            style.normal.textColor = buttonFontColor;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.background = (Texture2D)buttonBackground;
            style.hover.background = (Texture2D)buttonBackgroundOver;

            return style;
        }

        // Style for Search
        public static GUIStyle ButtonSearch()
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = (Texture2D)buttonSearch;
            style.hover.background = (Texture2D)buttonSearchOver;
            return style;
        }

        // Style for Selector List
        public static GUIStyle SelectorStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = textFieldFontSize;
            style.normal.textColor = textFieldFontColor;
            style.padding.left = textFieldPadding;
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.background = (Texture2D)selectorBackground;
            return style;
        }

        // Style for Large Selector List
        public static GUIStyle SelectorLargeStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = textFieldFontSize;
            style.normal.textColor = textFieldFontColor;
            style.padding.left = textFieldPadding;
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.background = (Texture2D)selectorLargeBackground;
            return style;
        }

        // Style for Combobox List
        public static GUIStyle ListStyle()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.onHover.background = (Texture2D)comboboxBackgroundSelected;
            style.hover.background = (Texture2D)comboboxBackground;
            style.padding.left = 1;
            style.padding.right = 1;
            style.padding.top = 1;
            style.padding.bottom = 1;
            return style;
        }

        // Style for Tab List
        public static GUIStyle AllButtonStyle(bool selected)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            if (selected)
                style.normal.background = (Texture2D)allButtonSelected;
            else
                style.normal.background = (Texture2D)allButton;

            style.onHover.background = (Texture2D)allButtonOver;
            style.hover.background = (Texture2D)allButtonOver;
            return style;
        }

        // Style for Tab List
        public static GUIStyle PluginButtonStyle(bool selected)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            if (selected)
                style.normal.background = (Texture2D)buttonOver;
            else
                style.normal.background = (Texture2D)button;

            style.onHover.background = (Texture2D)buttonOver;
            style.hover.background = (Texture2D)buttonOver;
            return style;
        }

        // Style for Tab List
        public static GUIStyle CategoryButtonStyle(Texture icon, Texture over)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.normal.background = (Texture2D)icon;
            style.onHover.background = (Texture2D)over;
            style.hover.background = (Texture2D)over;
            return style;
        }

        // Style for Tab List
        public static GUIStyle TabStyle(Texture background, Texture over)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.normal.background = (Texture2D)background;
            style.onHover.background = (Texture2D)over;
            style.hover.background = (Texture2D)over;
            style.padding.left = 1;
            style.padding.right = 1;
            style.padding.top = 1;
            style.padding.bottom = 1;
            return style;
        }


        /*
         * Methods to handle new plugins creation and editing
         * 
        */

        // Types of UI intefaces drag&drop components
        public enum UITypes
        {
            none,
            button,
            label,
            text,
            textField,
            intField,
            floatField,
            pos3D,
            gameobject,
            toggle,
            selector
        };

        // Toolbox content definition
        // What is here is what the Editor Toolbox will show up
        public static UITypes[] toolbox = new UITypes[10] {
        UITypes.textField,
        UITypes.intField,
        UITypes.floatField,
        UITypes.toggle,
        UITypes.selector,
        UITypes.label,
        UITypes.button,
        UITypes.text,
        UITypes.pos3D,
        UITypes.gameobject
        };

        // Draw a individual component at the tool box
        // Uses the original component look and fell
        public static void DrawToolboxItem(Rect position, UITypes type)
        {
            switch (type)
            {
                case UITypes.label:
                    DrawLabel(position, "Label");
                    break;
                case UITypes.button:
                    DrawButton(position, "Button");
                    break;
                case UITypes.text:
                    DrawText(position, "Text");
                    break;
                case UITypes.textField:
                    DrawField(position, "Text Field", "Text");
                    break;
                case UITypes.intField:
                    DrawField(position, "Int Field", 0);
                    break;
                case UITypes.floatField:
                    DrawField(position, "Float Field", 1.0f);
                    break;
                case UITypes.pos3D:
                    Draw3DPosition(position, "3D Field", new Vector3(0, 0, 0), 0.65f);
                    break;
                case UITypes.gameobject:
                    DrawGameObject(position, "GameObj", "", 0.65f);
                    break;
                case UITypes.toggle:
                    DrawToggleBox(position, "Toggle", false);
                    break;
                case UITypes.selector:
                    string[] options = new string[] { "~ none ~", "option1", "option2" };
                    DrawSelector(position, "Selector", 0, options);
                    break;
            }
        }

        /* 
         * Methods to Draw GUI Components 
         * 
         */

        public static bool DrawTabCreate(Rect position, bool selected)
        {
            return DrawTab(position, tabCreate, tabCreateOver, selected);
        }

        public static bool DrawTabEdit(Rect position, bool selected)
        {
            return DrawTab(position, tabEdit, tabEditOver, selected);
        }

        public static bool DrawTabDoc(Rect position, bool selected)
        {
            return DrawTab(position, tabDoc, tabDocOver, selected);
        }

        public static bool DrawTabRestore(Rect position, bool selected)
        {
            return DrawTab(position, tabRestore, tabRestoreOver, selected);
        }

        // Draw a Tab
        public static bool DrawTab(Rect position, Texture background, Texture over, bool selected)
        {
            Rect tab = new Rect(position.x, position.y, background.width, background.height);
            bool button = false;

            if (selected)
                button = GUI.Button(tab, "", TabStyle(over, over));
            else
                button = GUI.Button(tab, "", TabStyle(background, over));

            return button;
        }

        // Draw a Scroll bar	
        public static void DrawScrollBar(float positionLeft, float positionTop, float barHeight)
        {
            float pos = positionTop;
            float height = barHeight - ImagePack.scrollArrowUp.height - ImagePack.scrollArrowDown.height;
            Rect scrollbar = new Rect(positionLeft, pos, ImagePack.scrollArrowUp.width, ImagePack.scrollArrowUp.height);
            GUI.DrawTexture(scrollbar, ImagePack.scrollArrowUp);
            pos += ImagePack.scrollArrowUp.height;
            scrollbar = new Rect(positionLeft, pos, ImagePack.scrollCategory.width, height);
            GUI.DrawTexture(scrollbar, ImagePack.scrollBar);
            pos += height;
            scrollbar = new Rect(positionLeft, pos, ImagePack.scrollArrowDown.width, ImagePack.scrollArrowDown.height);
            GUI.DrawTexture(scrollbar, ImagePack.scrollArrowDown);
        }
        // Draw a Button
        public static bool DrawButton(float positionX, float positionY, string label)
        {
            return DrawButton(positionX, positionY, label, 1f);
        }

        // Draw a Button
        public static bool DrawButton(float positionX, float positionY, string label, float scale)
        {
            GUIStyle style = ButtonStyle();
            Vector2 size = style.CalcSize(new GUIContent(label));
            float buttonWidth = buttonBackground.width + scale;
            if (size.x + buttonMargin > buttonBackground.width + scale)
            {
                float s = Mathf.Round(size.x / 50)*50+50;
                buttonWidth =s;
            }
            Rect button = new Rect(positionX + buttonMargin, positionY, buttonWidth, buttonBackground.height);
            return GUI.Button(button, label, ButtonStyle());

        }

        // Draw a Button
        public static bool DrawButton(Rect position, string label)
        {
            GUIStyle style = ButtonStyle();
            Vector2 size = style.CalcSize(new GUIContent(label));
            float buttonWidth = buttonBackground.width ;
            if (size.x + buttonMargin > buttonBackground.width )
            {
                float s = Mathf.Round(size.x / 25) * 25 + 25;
                buttonWidth = s;
            }
            Rect button = new Rect(position.x + buttonMargin, position.y, buttonWidth, buttonBackground.height);
            return GUI.Button(button, label, ButtonStyle());
        }

        // Draw a Button
        public static bool DrawButton(Rect position, string label, Color color)
        {
            GUIStyle style = ButtonStyle();
            Vector2 size = style.CalcSize(new GUIContent(label));
            float buttonWidth = buttonBackground.width ;
            if (size.x > buttonBackground.width )
                buttonWidth = size.x;
            Rect button = new Rect(position.x + buttonMargin, position.y, buttonBackground.width, buttonBackground.height);
           // GUIStyle style = ButtonStyle();
            style.normal.textColor = color;
            return GUI.Button(button, label, style);
        }

        public static bool DrawButton(Rect position, string label, Color color, string tooltip)
        {
            Rect button = new Rect(position.x + buttonMargin, position.y, buttonBackground.width, buttonBackground.height);
            GUIStyle style = ButtonStyle();
            style.normal.textColor = color;

            return GUI.Button(button, new GUIContent(label,tooltip), style);
        }


        // Draw a Button
        public static bool DrawSmallButton(Rect position, string label)
        {
            return GUI.Button(position, label, ButtonStyle());
        }

        // Draw a Label
        public static void DrawLabel(float positionX, float positionY, string text)
        {
            Rect label = new Rect(positionX + labelMargin, positionY, labelWidth, labelHeight);
            GUI.Label(label, text, LabelStyle());
        }

        // Draw a Label
        public static void DrawLabel(Rect position, string text)
        {
            Rect label = new Rect(position.x + labelMargin, position.y, labelWidth, labelHeight);
            GUI.Label(label, text, LabelStyle());
        }

        // Draw a Text
        public static void DrawText(float positionX, float positionY, string text)
        {
            Rect label = new Rect(positionX + labelMargin, positionY, labelWidth, labelHeight);
            GUI.Label(label, text, TextStyle());
        }

        public static void DrawText(Rect position, string text)
        {
            GUI.Label(position, text, TextStyle());
        }

        public static void DrawText(Rect position, string text, string tooltip)
        {
            GUIContent content = new GUIContent(text, tooltip);
            GUI.Label(position, content, TextStyle());
            position.width += 40;
            GUIStyle style = TextStyle();
            Vector2 size = style.CalcSize(content);
            position.width = size.x;
            GUI.Button(position, content, TextStyle());
        }

        public static void DrawText(Rect position, string text, Color color)
        {
            GUIStyle style = TextStyle();
            style.normal.textColor = color;
            GUI.Label(position, text, style);
        }

        // Draw a Text
        public static float DrawMultilineText(Rect position, string text)
        {
            GUIContent theContent = new GUIContent(text);
            float textHeight = TextMultiLineStyle().CalcHeight(theContent, position.width);
         //   int numLines = (int)(textHeight / TextMultiLineStyle().lineHeight);
            position.y += textHeight/2;
            GUI.Label(position, theContent, TextMultiLineStyle());
           
            return textHeight;
        }

        // Draw a Field Saving Preferences
        public static string DrawSavedData(Rect position, string name, string field, bool mask)
        {
            Rect pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            string data1, data2;
            GUI.Label(position, name, FieldStyle());
            data1 = EditorPrefs.GetString(field);
            if (mask)
            {
                data2 = GUI.PasswordField(pos, data1, '*', TextFieldStyle());
            }
            else
            {
                data2 = EditorGUI.TextField(pos, data1, TextFieldStyle());
            }

            if (data2 != data1)
                EditorPrefs.SetString(field, data2);
            return data2;
        }

        // Draw a set of Label plus Wide TextField
        public static string DrawField(Rect position, string name, string field, float width, float height)
        {
            float maxWidth = 0.8f;
          //  int maxLength = 500;
            float fieldWidth = width * position.width;
            if (width > maxWidth)
                fieldWidth = maxWidth * position.width;
            GUI.Label(position, name, FieldStyle());
            Rect pos = new Rect(position.x + (position.width - fieldWidth), position.y, fieldWidth - fieldMargin, height);
            return EditorGUI.TextArea(pos, field, TextAreaStyle());
        }

        // Draw a set of Label plus Wide TextField
        public static string DrawField(Rect position, string name, string field, float width)
        {
            float maxWidth = 0.8f;
            float fieldWidth = width * position.width;
            if (width > maxWidth)
                fieldWidth = maxWidth * position.width;
            GUI.Label(position, name, FieldStyle());
            Rect pos = new Rect(position.x + (position.width - fieldWidth), position.y, fieldWidth - fieldMargin, position.height - lineSpace);
            return EditorGUI.TextField(pos, field, TextFieldStyle());
        }

        // Draw a set of Label plus Wide TextField
        public static string DrawField(Rect position, string field)
        {
            return EditorGUI.TextField(position, field, TextFieldStyle());
        }
      

        // Draw a set of Label plus TextField
        public static string DrawField(Rect position, string name, string field)
        {
            GUI.Label(position, name, FieldStyle());
            Rect pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            return EditorGUI.TextField(pos, field, TextFieldStyle());
        }

        // Draw a set of Label plus TextField that can do password masking
        public static string DrawField(Rect position, string name, string field, bool password)
        {
            GUI.Label(position, name, FieldStyle());
            Rect pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            if (password)
            {
                return GUI.PasswordField(pos, field, '*', TextFieldStyle());
            }
            else
            {
                return EditorGUI.TextField(pos, field, TextFieldStyle());
            }
        }

        // Draw a set of Label plus IntField
        public static int DrawField(Rect position, string name, int field)
        {
            GUI.Label(position, name, FieldStyle());
            Rect pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
           // int typedValue = 0;
            field = EditorGUI.IntField(pos, field, TextFieldStyle());
            return field;
            /*string userTyped = GUI.TextField(pos, field.ToString(), TextFieldStyle());
            if (userTyped == "")
                return 0;
            if (int.TryParse(userTyped, out typedValue))
                return typedValue;
            else
                return field;*/
        }

        public static long DrawField(Rect position, string name, long field)
        {
            GUI.Label(position, name, FieldStyle());
            Rect pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            // int typedValue = 0;
            field = EditorGUI.LongField(pos, field, TextFieldStyle());
            return field;
            /*string userTyped = GUI.TextField(pos, field.ToString(), TextFieldStyle());
            if (userTyped == "")
                return 0;
            if (int.TryParse(userTyped, out typedValue))
                return typedValue;
            else
                return field;*/
        }


        // Draw a set of Label plus FloatField
        public static float DrawField(Rect position, string name, float field)
        {
            GUI.Label(position, name, FieldStyle());
            Rect pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
           // float typedValue = 0;
         //   string typedField = field.ToString();
            /*if (!typedField.Contains("."))
            {
                typedField = typedField + ".";
            }*/
            field = EditorGUI.FloatField(pos, field, TextFieldStyle());
            return field;
            /*string userTyped = GUI.TextField(pos, typedField, TextFieldStyle());
            if (userTyped == "")
            {
                return 0f;
            }
            if (float.TryParse(userTyped, out typedValue))
            {
                return typedValue;
            }
            else
            {
                return field;
            }*/

        }

        // Draw a 3D Position Fields (Vector3: X, Y, Z)
        public static Vector3 Draw3DPosition(Rect position, string name, Vector3 field)
        {
            GUI.Label(position, name, FieldStyle());
            string fieldLabel = "Position in 3D Space:";
            Rect pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            return EditorGUI.Vector3Field(pos, fieldLabel, field);
        }

        // Draw a 3D Position Fields (Vector3: X, Y, Z)
        public static Vector3 Draw3DPosition(Rect position, string name, Vector3 field, float width)
        {
            float maxWidth = 0.8f;
            float fieldWidth = width * position.width;
            if (width > maxWidth)
                fieldWidth = maxWidth * position.width;

            GUI.Label(position, name, FieldStyle());
            string fieldLabel = "Position in 3D Space:";
            Rect pos = new Rect(position.x + (position.width - fieldWidth), position.y, fieldWidth - fieldMargin, position.height - lineSpace);
            return EditorGUI.Vector3Field(pos, fieldLabel, field);
        }


        // Draw a Object Field
        public static Object DrawObject(Rect position, string name, Object fieldObject)
        {
            GUI.Label(position, name, FieldStyle());
            bool AllowObjectFromScene = true;
            Rect pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            return EditorGUI.ObjectField(pos, fieldObject, typeof(Object), AllowObjectFromScene);
        }

        static bool iconSelection = false;
        // Draw a GameObject Field
        public static string DrawTextureAsset(Rect position, string name, string icon)
        {
            float maxWidth = 0.8f;
            float fieldWidth = maxWidth * position.width;
            position.x += 0.3f * position.width;

            Color tempColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            // Draw the label
            GUI.Label(position, name, FieldStyle());
          //  bool AllowObjectFromScene = false;
            // Create an null object
            Texture2D tempTexture = null;

            // Try to find the Game Object
            if (icon != "")
            {
                tempTexture = (Texture2D)AssetDatabase.LoadMainAssetAtPath(icon);
                //Debug.Log("Asset:"+tempTexture);
            }
            // Draw the field
            Rect pos = new Rect(position.x + (position.width - fieldWidth), position.y, fieldWidth / 2, fieldWidth / 2);
            //GameObject temp = (GameObject)EditorGUI.ObjectField(pos, tempTexture, typeof(GameObject), AllowObjectFromScene);
            //tempTexture = (Texture2D)EditorGUI.ObjectField(pos, tempTexture, typeof(Texture2D), AllowObjectFromScene);

            if (GUI.Button(pos, " Select an Icon", IconStyle()))
            {
                int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", controlID);
                iconSelection = true;
            }

            string commandName = Event.current.commandName;
            if (iconSelection)
            {
                if (commandName == "ObjectSelectorUpdated")
                {
                    tempTexture = (Texture2D)EditorGUIUtility.GetObjectPickerObject();
                }
                else if (commandName == "ObjectSelectorClosed")
                {
                    //tempTexture = (Texture2D)EditorGUIUtility.GetObjectPickerObject ();
                    iconSelection = false;
                }
            }

            GUI.backgroundColor = tempColor;
            // If there is a Game Object, return the name.
            if (tempTexture != null)
            {

                string path = AssetDatabase.GetAssetPath(tempTexture);
                EditorGUI.DrawPreviewTexture(pos, tempTexture);
                //Debug.Log("Path:"+path);
                return path;
            }
            // Else return blank
            return "";
        }

        public static string DrawSpriteAsset(Rect position, string name, string icon)
        {
            float maxWidth = 0.8f;
            float fieldWidth = maxWidth * position.width;
            position.x += 0.3f * position.width;

            Color tempColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            // Draw the label
            GUI.Label(position, name, FieldStyle());
        //    bool AllowObjectFromScene = false;
            // Create an null object
            Sprite tempTexture = null;
          //  string t = AssetDatabase.AssetPathToGUID(icon);
          //  Debug.Log(AssetDatabase.GUIDToAssetPath(guid2));
         //   Debug.Log(icon+" | "+t+" | "+ AssetDatabase.GUIDToAssetPath(t));
            // Try to find the Game Object
            if (icon != "")
            {
                Object asset = AssetDatabase.LoadAssetAtPath(icon, typeof(Sprite));
                if (asset is Sprite)
                    tempTexture = (Sprite)asset;
                if (tempTexture == null)
                {
                   
                    return " ";
                }
                //Debug.Log("Asset:"+tempTexture);
            }
            // Draw the field
            Rect pos = new Rect(position.x + (position.width - fieldWidth), position.y,100 /* fieldWidth / 2*/, 100 /*fieldWidth / 2*/);
            //GameObject temp = (GameObject)EditorGUI.ObjectField(pos, tempTexture, typeof(GameObject), AllowObjectFromScene);
            //tempTexture = (Texture2D)EditorGUI.ObjectField(pos, tempTexture, typeof(Texture2D), AllowObjectFromScene);

            if (GUI.Button(pos, " Select an Icon", IconStyle()))
            {
                int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, "", controlID);
                iconSelection = true;
            }

            string commandName = Event.current.commandName;
            if (iconSelection)
            {
                if (commandName == "ObjectSelectorUpdated")
                {
                    tempTexture = (Sprite)EditorGUIUtility.GetObjectPickerObject();
                    
                }
                else if (commandName == "ObjectSelectorClosed")
                {
                    //tempTexture = (Texture2D)EditorGUIUtility.GetObjectPickerObject ();
                    iconSelection = false;
                }
            }

            GUI.backgroundColor = tempColor;
            // If there is a Game Object, return the name.
            if (tempTexture != null)
            {

                string path = AssetDatabase.GetAssetPath(tempTexture);
                EditorGUI.DrawPreviewTexture(pos, tempTexture.texture);
                //Debug.Log("Path:"+path);
                return path;
            }
            else if (tempTexture == null && icon != "")
            {

                return " ";
            }
            // Else return blank
            return "";
        }

        // Draw a GameObject Field
        public static string DrawGameObject(Rect position, string name, string fieldObject, float width)
        {
            float maxWidth = 0.8f;
            float fieldWidth = width * position.width;
            if (width > maxWidth)
                fieldWidth = maxWidth * position.width;

            // Draw the label
            GUI.Label(position, name, FieldStyle());
            bool AllowObjectFromScene = true;
            Rect pos = new Rect(position.x + (position.width - fieldWidth), position.y, fieldWidth - fieldMargin, position.height - lineSpace);
            // Create an null object
            GameObject tempObject = null;

            // Try to find the Game Object
            if (fieldObject != "")
            {
                tempObject = (GameObject)AssetDatabase.LoadMainAssetAtPath(fieldObject);
                // If object null, means it is an Object from Scene
                if (tempObject == null)
                {
                    tempObject = GameObject.Find(fieldObject);
                    //Debug.Log("Scene:"+tempObject);
                }
                //Debug.Log("Asset:"+tempObject);
            }
            // Draw the field
            tempObject = (GameObject)EditorGUI.ObjectField(pos, tempObject, typeof(GameObject), AllowObjectFromScene);
            // If there is a Game Object, return the name.
            if (tempObject != null)
            {
                string path = AssetDatabase.GetAssetPath(tempObject);
                // If path blank, means it is an Object from Scene.
                if (path == "")
                {
                    // For Scene Objects get only the name.
                    path = tempObject.name;
                }
                //Debug.Log("Path:"+path);
                return path;
            }
            // Else return blank
            return "";
        }

        // Draw a GameObject Field
        public static GameObject DrawGameObject(Rect position, string name, GameObject fieldObject, float width)
        {
            float maxWidth = 0.8f;
            float fieldWidth = width * position.width;
            if (width > maxWidth)
                fieldWidth = maxWidth * position.width;

            // Draw the label
            GUI.Label(position, name, FieldStyle());
            bool AllowObjectFromScene = true;
            Rect pos = new Rect(position.x + (position.width - fieldWidth), position.y, fieldWidth - fieldMargin, position.height - lineSpace);
            // Create an null object

            // Draw the field
            fieldObject = (GameObject)EditorGUI.ObjectField(pos, fieldObject, typeof(GameObject), AllowObjectFromScene);
            // If there is a Game Object, return the name.
            return fieldObject;
        }


        // Draw a Checkbox 
        public static bool DrawToggleBox(Rect position, string name, bool field)
        {
            GUI.Label(position, name, FieldStyle());
            Rect pos = new Rect(position.x + 0.6f * position.width, position.y, checkboxBackground.width, checkboxBackground.height);
            Color temp = GUI.color;
            GUI.color = new Color(1, 1, 1, 0.0f);
            bool state = EditorGUI.Toggle(pos, field);
            GUI.color = temp;
            if (state)
                GUI.DrawTexture(pos, checkboxBackgroundSelected);
            else
                GUI.DrawTexture(pos, checkboxBackground);
            return state;
        }

        public static string DrawSelector(Rect position, string name, string selected, string[] list)
        {
            int index = 0;
            for (int i = 0; i < list.Length; i++)
            {
                if (selected == list[i])
                    index = i;
            }

            Rect pos = position;
            if (name != "")
            {
                GUI.Label(position, name, FieldStyle());
                pos = new Rect(position.x + position.width / 2, position.y+4, position.width / 2 - fieldMargin, position.height - lineSpace);
            }
            index = EditorGUI.Popup(pos, index, list, SelectorStyle());
            return list[index];

        }

        public static int DrawSelector(Rect position, string name, int selected, string[] list)
        {
            Rect pos = position;
            if (name != "")
            {
                GUI.Label(position, name, FieldStyle());
                pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            }
            selected = EditorGUI.Popup(pos, selected, list, SelectorStyle());
            return selected;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="selected"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string DrawCombobox(Rect position, string name, string selected, string[] list)
        {
            int index = 0;
            for (int i = 0; i < list.Length; i++)
            {
                if (selected == list[i])
                    index = i;
            }

            Rect pos = position;
            if (name != "")
            {
                GUI.Label(position, name, FieldStyle());
                pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            }
            index = EditorGUI.Popup(pos, index, list, SelectorLargeStyle());
            return list[index];

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="selected"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int DrawCombobox(Rect position, string name, int selected, string[] list)
        {
            Rect pos = position;
            if (name != "")
            {
                GUI.Label(position, name, FieldStyle());
                pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            }
            else
            {
                pos.height -= lineSpace;
            }
            selected = EditorGUI.Popup(pos, selected, list, SelectorLargeStyle());

            return selected;
        }

        /// <summary>
        /// Draw a Combobox
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="selected"></param>
        /// <param name="listBox"></param>
        /// <returns></returns>
        public static int DrawCombobox(Rect position, string name, int selected, Combobox listBox)
        {
            Rect pos = position;

            if (name != "")
            {
                GUI.Label(position, name, FieldStyle());
                pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            }
            else
            {
                pos.height -= lineSpace;
            }

            if (GUI.Button(pos, listBox.listBox[selected], "button"))
            {
                listBox.ShowList();
            }
            else
            {
                if (Event.current.clickCount > 0)
                    listBox.HideList();
            }

            return listBox.RenderList(pos);
        }

        public static int DrawMaskField(Rect position, string name, int selected, string[] options)
        {
            Rect pos = position;
            if (name != "")
            {
                GUI.Label(position, name, FieldStyle());
                pos = new Rect(position.x + position.width / 2, position.y, position.width / 2 - fieldMargin, position.height - lineSpace);
            }
            else
            {
                pos.height -= lineSpace;
            }
            selected = EditorGUI.MaskField(pos, selected, options);

            return selected;
        }

        public class ItemTemp
        {
            public int mIndex;
            public string mName;
            public bool mSelected;

            ItemTemp(int index, string name, bool selected) { mIndex = index; mName = name; mSelected = selected; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="itemlist"></param>
            /// <param name="selectedItem"></param>
            /// <returns></returns>
            public static List<ItemTemp> makeList(string[] itemlist, int selectedItem)
            {
                List<ItemTemp> wTempList = new List<ItemTemp>();
                for (int i = 0; i < itemlist.Length; i++)
                {
                    //     Debug.LogError("ItemTemp makeList  "+ itemlist[i]+ " "+(i == selectedItem));
                    wTempList.Add(new ItemTemp(i, itemlist[i], i == selectedItem));
                }
                return wTempList;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="iItemlist"></param>
            /// <returns></returns>
            public static int getSubListSelectedItemIndex(List<ItemTemp> iItemlist)
            {
                for (int i = 0; i < iItemlist.Count; i++)
                {
                    //   Debug.LogError("ItemTemp makeList  " + iItemlist[i].mIndex +" "+iItemlist[i].mName + " " + iItemlist[i].mSelected);

                    if (iItemlist[i].mSelected)
                    {
                        return i;
                    }
                }
                return 0;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="iItemlist"></param>
            /// <returns></returns>
            public static string[] getStrings(List<ItemTemp> iItemlist)
            {
                //string[] wTemp;
                List<string> wTemp = new List<string>();
                foreach (ItemTemp wItem in iItemlist)
                {
                    wTemp.Add(wItem.mName);
                }
                return wTemp.ToArray();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="iItemList"></param>
            /// <param name="iSubListIndex"></param>
            /// <returns></returns>
            public static int getOrignalItemIndex(List<ItemTemp> iItemList, int iSubListIndex)
            {
                if (iSubListIndex >= iItemList.Count)
                {
                    return 0;
                }
                return iItemList[iSubListIndex].mIndex;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="iItemList"></param>
            /// <param name="iSubListIndex"></param>
            /// <returns></returns>
            public static string getOrignalItemName(List<ItemTemp> iItemList, int iSubListIndex)
            {
                if (iSubListIndex >= iItemList.Count)
                {
                    return "";
                }
                return iItemList[iSubListIndex].mName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="selected"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int DrawDynamicPartialListSelector(Rect position, string label, ref string name, int selected, string[] list)
        {
            GUI.Label(position, label, FieldStyle());
            //Show the box used to search the list.
            Rect searchBoxPosition = new Rect(position.x + position.width / 2, position.y, 100, position.height - lineSpace);
            name = EditorGUI.TextField(searchBoxPosition, name, TextFieldStyle());

            Rect wComboBoxPosition = new Rect(position.x + 110 + position.width / 2, position.y, position.width - 10, position.height - lineSpace);

            string wSearched = name.ToLower();
            List<ItemTemp> wFilteredItems = new List<ItemTemp>();
            if (name.Length > 1)
            {
                List<ItemTemp> wItems = ItemTemp.makeList(list, selected);
                wFilteredItems = wItems.FindAll(
                    delegate (ItemTemp wTemp)
                    {
                        return wTemp.mName.ToLower().Contains(wSearched); //Problem, case sensitive.
                }
                    );
            }
            else { wFilteredItems = ItemTemp.makeList(list, selected); }
            int wSelectedinSublist = EditorGUI.Popup(wComboBoxPosition, ItemTemp.getSubListSelectedItemIndex(wFilteredItems), ItemTemp.getStrings(wFilteredItems), SelectorLargeStyle());
            return ItemTemp.getOrignalItemIndex(wFilteredItems, wSelectedinSublist);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="selected"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int DrawDynamicPartialListSelector(Rect position, string label, ref string name, int selected, List<string> listorg)
        {
            GUI.Label(position, label, FieldStyle());
            string[] list = listorg.ToArray();
            //Show the box used to search the list.
            Rect searchBoxPosition = new Rect(position.x + position.width / 2, position.y, 100, position.height - lineSpace);
            name = EditorGUI.TextField(searchBoxPosition, name, TextFieldStyle());

            Rect wComboBoxPosition = new Rect(position.x + 110 + position.width / 2, position.y, position.width - 10, position.height - lineSpace);

            string wSearched = name.ToLower();
            List<ItemTemp> wFilteredItems = new List<ItemTemp>();
            if (name.Length > 1)
            {
                List<ItemTemp> wItems = ItemTemp.makeList(list, selected);
                wFilteredItems = wItems.FindAll(
                    delegate (ItemTemp wTemp)
                    {
                        return wTemp.mName.ToLower().Contains(wSearched); //Problem, case sensitive.
                    }
                    );
            }
            else { wFilteredItems = ItemTemp.makeList(list, selected); }
            int wSelectedinSublist = EditorGUI.Popup(wComboBoxPosition, ItemTemp.getSubListSelectedItemIndex(wFilteredItems), ItemTemp.getStrings(wFilteredItems), SelectorLargeStyle());
          //  Debug.LogError("wSelectedinSublist=" + wSelectedinSublist);
            if (ItemTemp.getStrings(wFilteredItems).Length == 0)
                return -1;
            return ItemTemp.getOrignalItemIndex(wFilteredItems, wSelectedinSublist);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="selected"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int DrawDynamicFilteredListSelector(Rect position, string label, ref string name, int selected, string[] list)
        {
            GUI.Label(position, label, FieldStyle());
            Rect wComboBoxPosition = new Rect(position.x + position.width / 2, position.y, position.width - 10, position.height - lineSpace);

            string wSearched = name.ToLower();
            //  Debug.LogWarning("selected " + selected+" "+ list[selected]);
            bool searchIn = false;
            int wSelectedinSublist = 0;
            List<ItemTemp> wFilteredItems = new List<ItemTemp>();
            if (name.Length > 1)
            {
                List<ItemTemp> wItems = ItemTemp.makeList(list, selected);
                wFilteredItems = wItems.FindAll(
                   delegate (ItemTemp wTemp)
                   {
                       return wTemp.mName.ToLower().Contains(wSearched); //Problem, case sensitive.
               }
                   );

                wSelectedinSublist = EditorGUI.Popup(wComboBoxPosition, ItemTemp.getSubListSelectedItemIndex(wFilteredItems), ItemTemp.getStrings(wFilteredItems), SelectorLargeStyle());
                //    Debug.LogWarning("name.Length > 1==| "+wSelectedinSublist+" = "+ ItemTemp.getOrignalItemIndex(wFilteredItems, wSelectedinSublist));
                searchIn = true;
            }
            else
            {
                wSelectedinSublist = EditorGUI.Popup(wComboBoxPosition, selected, list, SelectorLargeStyle());
                //    Debug.LogWarning("name.Length 0 | "+wSelectedinSublist);
            }

            //Show the box used to search the list.
            Rect searchBoxPosition = new Rect(position.x + position.width * 1.5f, position.y, position.width * 0.4f, position.height - lineSpace);
            name = EditorGUI.TextField(searchBoxPosition, name, FilterFieldStyle());

            if (searchIn)
                return ItemTemp.getOrignalItemIndex(wFilteredItems, wSelectedinSublist);
            else
                return wSelectedinSublist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="selected"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string DrawDynamicFilteredListSelector(Rect position, string label, ref string name, string selected, string[] list)
        {
            GUI.Label(position, label, FieldStyle());
            Rect wComboBoxPosition = new Rect(position.x + position.width / 2, position.y, position.width - 10, position.height - lineSpace);

            int index = 0;
            for (int i = 0; i < list.Length; i++)
            {
                if (selected == list[i])
                    index = i;
            }

            string wSearched = name.ToLower();
            List<ItemTemp> wFilteredItems = new List<ItemTemp>();
            if (name.Length > 2)
            {
                List<ItemTemp> wItems = ItemTemp.makeList(list, index);
                wFilteredItems = wItems.FindAll(
                    delegate (ItemTemp wTemp)
                    {
                        return wTemp.mName.ToLower().Contains(wSearched); //Problem, case sensitive.
                }
                    );
            }
            else { wFilteredItems = ItemTemp.makeList(list, index); }
            int wSelectedinSublist = EditorGUI.Popup(wComboBoxPosition, ItemTemp.getSubListSelectedItemIndex(wFilteredItems), ItemTemp.getStrings(wFilteredItems), SelectorLargeStyle());

            //Show the box used to search the list.
            Rect searchBoxPosition = new Rect(position.x + position.width * 1.5f, position.y, /*100*/ position.width * 0.4f, position.height - lineSpace);
            name = EditorGUI.TextField(searchBoxPosition, name, FilterFieldStyle());

            return ItemTemp.getOrignalItemName(wFilteredItems, wSelectedinSublist);
        }

    }
    // Class to create a custom combobox
    public class Combobox
    {

        public string[] listBox;            // The list to store
        public bool showList = false;       // Show list flag
        public bool hideList = false;       // Hide list flag
        public int selected = 0;            // Item selected 
        public int closeDelay = 30;         // Close delay - critical due to mouse noise
        public int closeCounter = 0;        // Close Timer

        // Instantiate the Combobox
        public Combobox(string[] list)
        {
            listBox = new string[list.Length];
            listBox = list;
        }

        // Show the list
        public void ShowList()
        {
            showList = true;
        }

        // Hide the list
        public void HideList()
        {
            if (showList)
                hideList = true;
        }

        // Render the Combobox
        public int RenderList(Rect pos)
        {
            // Render the list if showList active
            if (showList)
            {
                Rect listRect = new Rect(pos.x, pos.y + 20, pos.width, 20 * listBox.Length);
                GUI.Box(listRect, "", "box");
                selected = GUI.SelectionGrid(listRect, selected, listBox, 1, ImagePack.ListStyle());
                // If to close the list, wait until the SelectionGrid get the mouse click.
                if ((hideList) && (closeCounter == 0))
                {
                    closeCounter = closeDelay;
                }
            }

            // Time to handle SelectionGrid mouse capture delay
            if (closeCounter > 0)
            {
                closeCounter--;
                if (closeCounter == 0)
                {
                    showList = false;
                    hideList = false;
                }
            }

            return selected;

        }
    }
}