namespace HNGamers
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    namespace Atavism
    {
        public class AtavismColorPicker : MonoBehaviour
        {
            public Canvas eyeCanvas;
            public Canvas eyebrowCanvas;
            public Canvas hairCanvas;
            public Canvas beardCanvas;
            public Canvas scarCanvas;
            public Canvas skinCanvas;
            public Canvas bodyartCanvas;
            public Canvas stubbleCanvas;
            public Canvas mouthCanvas;

            public Canvas primaryCanvas;
            public Canvas secondaryCanvas;
            public Canvas tertiaryCanvas;
            public Canvas metalPrimaryCanvas;
            public Canvas metalSecondaryCanvas;
            public Canvas metalDarkCanvas;
            public Canvas leatherPrimaryCanvas;
            public Canvas leatherSecondaryCanvas;
            public Canvas leatherTertiaryCanvas;

            public TextMeshProUGUI bodyPartTextMeshProUGUI;
            private List<Canvas> canvases = new List<Canvas>();
            int currentUI = -1;
            public int currentBodyPart = 0;
            public string[] bodyParts;
            private void Start()
            {
                if (bodyPartTextMeshProUGUI)
                {
                    bodyPartTextMeshProUGUI.text = bodyParts[currentBodyPart].ToString();
                }

                if (eyeCanvas)
                {
                    canvases.Add(eyeCanvas);
                    eyeCanvas.enabled = true;
                }

                if (eyebrowCanvas)
                {
                    canvases.Add(eyebrowCanvas);
                    eyebrowCanvas.enabled = false;
                }

                if (hairCanvas)
                {
                    canvases.Add(hairCanvas);
                    hairCanvas.enabled = false;
                }

                if (beardCanvas)
                {
                    canvases.Add(beardCanvas);
                    beardCanvas.enabled = false;
                }

                if (scarCanvas)
                {
                    canvases.Add(scarCanvas);
                    scarCanvas.enabled = false;
                }

                if (skinCanvas)
                {
                    canvases.Add(skinCanvas);
                    skinCanvas.enabled = false;
                }

                if (bodyartCanvas)
                {
                    canvases.Add(bodyartCanvas);
                    bodyartCanvas.enabled = false;
                }

                if (stubbleCanvas)
                {
                    canvases.Add(stubbleCanvas);
                    stubbleCanvas.enabled = false;
                }

                if (mouthCanvas)
                {
                    canvases.Add(mouthCanvas);
                    mouthCanvas.enabled = false;
                }

                if (primaryCanvas)
                {
                    canvases.Add(primaryCanvas);
                    primaryCanvas.enabled = false;
                }

                if (secondaryCanvas)
                {
                    canvases.Add(secondaryCanvas);
                    secondaryCanvas.enabled = false;
                }

                if (tertiaryCanvas)
                {
                    canvases.Add(tertiaryCanvas);
                    tertiaryCanvas.enabled = false;
                }

                if (metalPrimaryCanvas)
                {
                    canvases.Add(metalPrimaryCanvas);
                    metalPrimaryCanvas.enabled = false;
                }

                if (metalSecondaryCanvas)
                {
                    canvases.Add(metalSecondaryCanvas);
                    metalSecondaryCanvas.enabled = false;
                }

                if (metalDarkCanvas)
                {
                    canvases.Add(metalDarkCanvas);
                    metalDarkCanvas.enabled = false;
                }

                if (leatherPrimaryCanvas)
                {
                    canvases.Add(leatherPrimaryCanvas);
                    leatherPrimaryCanvas.enabled = false;
                }

                if (leatherSecondaryCanvas)
                {
                    canvases.Add(leatherSecondaryCanvas);
                    leatherSecondaryCanvas.enabled = false;
                }

                if (leatherTertiaryCanvas)
                {
                    canvases.Add(leatherTertiaryCanvas);
                    leatherTertiaryCanvas.enabled = false;
                }

                currentUI = 0;
            }

            public void togglebodyPart()
            {
                currentBodyPart++;
                if (currentBodyPart > bodyParts.Length - 1)
                    currentBodyPart = 0;
                bodyPartTextMeshProUGUI.text = bodyParts[currentBodyPart].ToString();
            }

            private void Update()
            {
            }

            public void toggleUi()
            {

                currentUI++;
                if (currentUI > 17)
                { currentUI = 0; }
                switch (currentUI)
                {
                    case 0:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = true;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 1:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = true;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 2:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = true;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 3:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = true;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 4:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = true;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 5:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = true;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 6:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = true;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 7:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = true;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 8:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = true;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;

                    case 9:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = true;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 10:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = true;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 11:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = true;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 12:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = true;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 13:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = true;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 14:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = true;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 15:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = true;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 16:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = true;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = false;
                        }
                        break;
                    case 17:
                        if (eyeCanvas)
                        {
                            eyeCanvas.enabled = false;
                        }
                        if (eyebrowCanvas)
                        {
                            eyebrowCanvas.enabled = false;
                        }
                        if (hairCanvas)
                        {
                            hairCanvas.enabled = false;
                        }
                        if (beardCanvas)
                        {
                            beardCanvas.enabled = false;
                        }
                        if (scarCanvas)
                        {
                            scarCanvas.enabled = false;
                        }
                        if (skinCanvas)
                        {
                            skinCanvas.enabled = false;
                        }
                        if (bodyartCanvas)
                        {
                            bodyartCanvas.enabled = false;
                        }
                        if (stubbleCanvas)
                        {
                            stubbleCanvas.enabled = false;
                        }
                        if (mouthCanvas)
                        {
                            mouthCanvas.enabled = false;
                        }
                        if (primaryCanvas)
                        {
                            primaryCanvas.enabled = false;
                        }
                        if (secondaryCanvas)
                        {
                            secondaryCanvas.enabled = false;
                        }
                        if (tertiaryCanvas)
                        {
                            tertiaryCanvas.enabled = false;
                        }
                        if (metalPrimaryCanvas)
                        {
                            metalPrimaryCanvas.enabled = false;
                        }
                        if (metalSecondaryCanvas)
                        {
                            metalSecondaryCanvas.enabled = false;
                        }
                        if (metalDarkCanvas)
                        {
                            metalDarkCanvas.enabled = false;
                        }
                        if (leatherPrimaryCanvas)
                        {
                            leatherPrimaryCanvas.enabled = false;
                        }
                        if (leatherSecondaryCanvas)
                        {
                            leatherSecondaryCanvas.enabled = false;
                        }
                        if (leatherTertiaryCanvas)
                        {
                            leatherTertiaryCanvas.enabled = true;
                        }
                        break;
                }
            }
        }
    }
}