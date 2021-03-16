using ADOFAI;
using DG.Tweening;
using HarmonyLib;
using SFB;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BetterEditor.Core.Patches
{
	[HarmonyPatch(typeof(scnEditor), "Start")]
	static class scnEditor_Start_Patch
    {
		static void Postfix(scnEditor __instance)
        {
			scnEditorPrivates.Setup();
			scnEditorPrivates.instance = __instance;
		}
    }

	[HarmonyPatch(typeof(scnEditor), "Update")]
	static class scnEditor_Update_Patch
    {
        static bool Prefix(scnEditor __instance)
        {
			__instance.autoImage.gameObject.SetActive(!GCS.standaloneLevelMode);
			if (GCS.standaloneLevelMode)
			{
				__instance.levelEditorCanvas.enabled = false;
				return false;
			}
			__instance.thumbnailMaker.gameObject.SetActive(true);
			if (StandaloneFileBrowser.lastFrameCount == Time.frameCount)
			{
				return false;
			}
			scnEditorPrivates.InvokeMethod("UpdateSteamCallbacks");

			scnEditorPrivates.InvokeMethod("UpdateSelectedFloor");

			__instance.levelEditorCanvas.enabled = !(bool) scnEditorPrivates.GetProperty("playMode");

			__instance.gameCanvas.enabled = (bool) scnEditorPrivates.GetProperty("playMode");

			List<scrFloor> temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

			__instance.playPause.interactable = temporaryFloors.Count() != 1;

			__instance.playPauseIcon.sprite = (bool) scnEditorPrivates.GetProperty("paused") ? __instance.playButtonIcon : __instance.pauseButtonIcon;

			scnEditorPrivates.InvokeMethod("OttoUpdate");

			if ((bool) scnEditorPrivates.GetField("refreshBgSprites"))
			{
				__instance.UpdateBackgroundSprites();
			}
			if ((bool) scnEditorPrivates.GetField("refreshDecSprites"))
			{
				__instance.UpdateDecorationSprites();
			}
			if (Input.GetKeyDown(KeyCode.Escape) && !GCS.standaloneLevelMode)
			{
				if (!(bool) scnEditorPrivates.GetProperty("paused"))
				{
					__instance.SwitchToEditMode(false);
					return false;
				}
				if (__instance.selectedFloor != null || !(bool) scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
				{
					scnEditorPrivates.InvokeMethod("DeselectFloors");
				}
				else
				{
					__instance.ToggleFileActionsPanel();
				}
			}
			if (!ScrollEvent.inside)
			{
				Vector2 mouseScrollDelta = Input.mouseScrollDelta;
				scrCamera scrCamera = scrCamera.instance;
				if (Mathf.Abs(mouseScrollDelta.y) > 0.05f)
				{
					float value = scrCamera.userSizeMultiplier - mouseScrollDelta.y * __instance.scrollSpeed;
					scrCamera.userSizeMultiplier = Mathf.Clamp(value, 0.5f, 15f);
				}
			}
			if ((bool) scnEditorPrivates.GetProperty("playMode"))
			{
				return false;
			}

			bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			bool flag2 = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
			bool key = Input.GetKey(KeyCode.BackQuote);
			bool flag3 = flag || flag2;
			bool flag4 = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

			scrFloor temporaryFloor;

			if (!(bool) scnEditorPrivates.GetProperty("userIsEditingAnInputField") && !(bool) scnEditorPrivates.GetField("showingPopup"))
			{
				BEKeybind.OnUpdate(__instance);

				// ----------------------------------------------------------------------------------------- start of keybinding
				if (!flag3)
				{
					if (Input.GetKeyDown(KeyCode.P))
					{
						__instance.Play();
					}
					else if (Input.GetKeyDown(KeyCode.LeftBracket))
					{
						__instance.ShowPrevPage();
					}
					else if (Input.GetKeyDown(KeyCode.RightBracket))
					{
						__instance.ShowNextPage();
					}
					else if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
					{
						scrCamera scrCamera2 = scrCamera.instance;

						float value2 = scrCamera2.userSizeMultiplier - 0.5f;
						scrCamera2.userSizeMultiplier = Mathf.Clamp(value2, 0.5f, 15f);
					}
					else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
					{
						scrCamera scrCamera3 = scrCamera.instance;
						float value3 = scrCamera3.userSizeMultiplier + 0.5f;
						scrCamera3.userSizeMultiplier = Mathf.Clamp(value3, 0.5f, 15f);
					}
					else if (Input.GetKeyDown(KeyCode.Home))
					{
						scnEditorPrivates.InvokeMethod("SelectFirstFloor");
					}
					else if (Input.GetKeyDown(KeyCode.End))
					{
						temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

						scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
							temporaryFloors[
								temporaryFloors.Count() - 1
							],
							true
						});
					}
					else if (Input.GetKeyDown(KeyCode.F1))
					{
						__instance.ShowPopup(true, scnEditor.PopupType.CopyrightWarning, true);
					}
					if (__instance.selectedFloor != null)
					{
						if (Input.GetKeyDown(KeyCode.LeftArrow))
						{
							temporaryFloor = scnEditorPrivates.InvokeMethod<scrFloor>("PreviousFloor", new object[] { __instance.selectedFloor });

							scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
								temporaryFloor,
								true
							});
						}
						else if (Input.GetKeyDown(KeyCode.RightArrow))
						{
							temporaryFloor = scnEditorPrivates.InvokeMethod<scrFloor>("NextFloor", new object[] { __instance.selectedFloor });

							scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
								temporaryFloor,
								true
							});
						}
						else if (Input.GetKeyDown(KeyCode.Backspace))
						{
							int seqID = __instance.selectedFloor.seqID;
							if ((bool)scnEditorPrivates.InvokeMethod("DeleteFloor", new object[] { seqID, true }))
							{
								temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

								scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
									temporaryFloors[seqID - 1],
									true
								});
							}
						}
						else if (Input.GetKeyDown(KeyCode.Delete))
						{
							int seqID2 = __instance.selectedFloor.seqID;
							if ((bool)scnEditorPrivates.InvokeMethod("DeleteFloor", new object[] { seqID2 + 1, true }))
							{
								temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

								scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
									temporaryFloors[seqID2],
									true
								});
							}
						}
						else if (Input.GetKeyDown(KeyCode.Alpha1))
						{
							switch ((int)scnEditorPrivates.GetField("currentPage"))
							{
								case 0:
									__instance.AddEventAtSelected(LevelEventType.SetSpeed);
									break;
								case 1:
									__instance.AddEventAtSelected(LevelEventType.Flash);
									break;
								case 2:
									__instance.AddEventAtSelected(LevelEventType.ShakeScreen);
									break;
							}
						}
						else if (Input.GetKeyDown(KeyCode.Alpha2))
						{
							switch ((int)scnEditorPrivates.GetField("currentPage"))
							{
								case 0:
									__instance.AddEventAtSelected(LevelEventType.Twirl);
									break;
								case 1:
									__instance.AddEventAtSelected(LevelEventType.MoveCamera);
									break;
								case 2:
									__instance.AddEventAtSelected(LevelEventType.SetPlanetRotation);
									break;
							}
						}
						else if (Input.GetKeyDown(KeyCode.Alpha3))
						{
							switch ((int)scnEditorPrivates.GetField("currentPage"))
							{
								case 0:
									__instance.AddEventAtSelected(LevelEventType.Checkpoint);
									break;
								case 1:
									__instance.AddEventAtSelected(LevelEventType.SetHitsound);
									break;
								case 2:
									__instance.AddEventAtSelected(LevelEventType.MoveDecorations);
									break;
							}
						}
						else if (Input.GetKeyDown(KeyCode.Alpha4))
						{
							switch ((int)scnEditorPrivates.GetField("currentPage"))
							{
								case 0:
									__instance.AddEventAtSelected(LevelEventType.CustomBackground);
									break;
								case 1:
									__instance.AddEventAtSelected(LevelEventType.RecolorTrack);
									break;
								case 2:
									__instance.AddEventAtSelected(LevelEventType.PositionTrack);
									break;
							}
						}
						else if (Input.GetKeyDown(KeyCode.Alpha5))
						{
							switch ((int)scnEditorPrivates.GetField("currentPage"))
							{
								case 0:
									__instance.AddEventAtSelected(LevelEventType.ColorTrack);
									break;
								case 1:
									__instance.AddEventAtSelected(LevelEventType.MoveTrack);
									break;
								case 2:
									__instance.AddEventAtSelected(LevelEventType.RepeatEvents);
									break;
							}
						}
						else if (Input.GetKeyDown(KeyCode.Alpha6))
						{
							switch ((int)scnEditorPrivates.GetField("currentPage"))
							{
								case 0:
									__instance.AddEventAtSelected(LevelEventType.AnimateTrack);
									break;
								case 1:
									__instance.AddEventAtSelected(LevelEventType.SetFilter);
									break;
								case 2:
									__instance.AddEventAtSelected(LevelEventType.Bloom);
									break;
							}
						}
						else if (Input.GetKeyDown(KeyCode.Alpha7))
						{
							switch ((int)scnEditorPrivates.GetField("currentPage"))
							{
								case 0:
									__instance.AddEventAtSelected(LevelEventType.AddDecoration);
									break;
								case 1:
									__instance.AddEventAtSelected(LevelEventType.HallOfMirrors);
									break;
								case 2:
									__instance.AddEventAtSelected(LevelEventType.SetConditionalEvents);
									break;
							}
						}
						else if (Input.GetKeyDown(KeyCode.F5))
						{
							char floorType = '5';
							for (int i = 0; i < 5; i++)
							{
								scnEditorPrivates.InvokeMethod("CreateFloor", new object[] {
									floorType, true, false
								});
							}
						}
						else if (Input.GetKeyDown(KeyCode.F7))
						{
							char floorType2 = '7';
							for (int j = 0; j < 7; j++)
							{
								scnEditorPrivates.InvokeMethod("CreateFloor", new object[] {
									floorType2, true, false
								});
							}
						}
					}
					else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
					{
						if (Input.GetKeyDown(KeyCode.LeftArrow))
						{
							temporaryFloor = scnEditorPrivates.InvokeMethod<scrFloor>("PreviousFloor", new object[] { __instance.multiSelectedFloors.First() });

							scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
								temporaryFloor,
								true
							});
						}
						if (Input.GetKeyDown(KeyCode.RightArrow))
						{
							temporaryFloor = scnEditorPrivates.InvokeMethod<scrFloor>("NextFloor", new object[] { __instance.multiSelectedFloors.Last() });

							scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
								temporaryFloor,
								true
							});
						}
						if (Input.GetKeyDown(KeyCode.Backspace))
						{
							scnEditorPrivates.InvokeMethod("DeleteMultiSelection", new object[] {
								true
							});
						}
						else if (Input.GetKeyDown(KeyCode.Delete))
						{
							scnEditorPrivates.InvokeMethod("DeleteMultiSelection", new object[] {
								false
							});
						}
					}
					else if (Input.GetKeyDown(KeyCode.A))
					{
						scnEditorPrivates.InvokeMethod("ToggleAuto");
					}
					if (Input.GetKeyDown(KeyCode.Escape))
					{
						scnEditorPrivates.InvokeMethod("DeselectFloors");
					}
					if (!(bool)scnEditorPrivates.GetField("showingFileActions") && (bool)scnEditorPrivates.GetField("hoveringFileActions") != (bool)scnEditorPrivates.InvokeMethod("CheckPointerInFilePanel"))
					{
						scnEditorPrivates.SetField("hoveringFileActions", scnEditorPrivates.InvokeMethod("CheckPointerInFilePanel"));
						if ((bool)scnEditorPrivates.InvokeMethod("CheckPointerInFilePanel"))
						{
							__instance.fileIcon.color = Color.white.WithAlpha(0.5f);
							__instance.fileIcon.DOColor(Color.white.WithAlpha(1f), 0.25f).SetUpdate(true);
							__instance.fileArrow.color = Color.white.WithAlpha(0.5f);
							__instance.fileArrow.DOColor(Color.white.WithAlpha(1f), 0.25f).SetUpdate(true);
						}
						else
						{
							__instance.fileIcon.color = Color.white.WithAlpha(1f);
							__instance.fileIcon.DOColor(Color.white.WithAlpha(0.5f), 0.25f).SetUpdate(true);
							__instance.fileArrow.color = Color.white.WithAlpha(1f);
							__instance.fileArrow.DOColor(Color.white.WithAlpha(0.5f), 0.25f).SetUpdate(true);
						}
					}
				}
				else if (flag)
				{
					if (flag2)
					{
						if (Input.GetKeyDown(KeyCode.S))
						{
							scnEditorPrivates.InvokeMethod("DeselectAnyUIGameObject");
							scnEditorPrivates.InvokeMethod("SaveLevelAs");
						}
						else if (Input.GetKeyDown(KeyCode.O))
						{
							scnEditorPrivates.InvokeMethod("DeselectAnyUIGameObject");
							scnEditorPrivates.InvokeMethod("OpenRecent");
						}
						else if (Input.GetKeyDown(KeyCode.L))
						{
							if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
							{
								scnEditorPrivates.InvokeMethod("FlipSelection", new object[] { false });
							}
							else if (__instance.selectedFloor != null)
							{
								scnEditorPrivates.InvokeMethod("FlipFloor", new object[] { __instance.selectedFloor, false, true });
							}
						}
						else if (Input.GetKeyDown(KeyCode.LeftArrow)) // multi-selection
						{
							temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

							if (__instance.selectedFloor != null)
							{
								scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
									__instance.selectedFloor,
									temporaryFloors[0],
									true
								});
							}
							else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
							{
								scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
									temporaryFloors[0],
									__instance.multiSelectPoint,
									true
								});
							}
						}
						else if (Input.GetKeyDown(KeyCode.RightArrow))
						{
							temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

							if (__instance.selectedFloor != null)
							{
								scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
									__instance.selectedFloor,
									temporaryFloors[ temporaryFloors.Count - 1 ],
									true
								});
							}
							else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
							{
								scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
									temporaryFloors[ temporaryFloors.Count - 1 ],
									__instance.multiSelectPoint,
									false
								});
							}
						}
						else if (Input.GetKeyDown(KeyCode.V) && __instance.clipboardCharsEvents.Any())
						{
							if (__instance.selectedFloor != null)
							{
								if (__instance.clipboardCharsEvents.Count() == 1)
								{
									List<LevelEvent> item = __instance.clipboardCharsEvents[0].Item2;
									scnEditorPrivates.InvokeMethod("PasteEvents", new object[] {
										__instance.selectedFloor, item, false, true
									});
								}
								else
								{
									int seqID3 = __instance.selectedFloor.seqID;

									scnEditorPrivates.InvokeMethod("PasteEvents", new object[] {
										__instance.selectedFloor, __instance.clipboardCharsEvents[0].Item2, false, true
									});

									for (int k = 1; k < __instance.clipboardCharsEvents.Count; k++)
									{
										temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

										if (seqID3 + k >= temporaryFloors.Count)
										{
											break;
										}

										scnEditorPrivates.InvokeMethod("PasteEvents", new object[] {
											temporaryFloors[seqID3 + k], __instance.clipboardCharsEvents[k].Item2, false, false
										});
									}
								}
							}
							else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty") && __instance.clipboardCharsEvents.Count() == 1 && __instance.multiSelectedFloors[0].seqID != 0)
							{
								List<LevelEvent> item2 = __instance.clipboardCharsEvents[0].Item2;

								scnEditorPrivates.InvokeMethod("PasteEvents", new object[] {
									__instance.multiSelectedFloors[0], item2, false, true
								});
							}
						}
					}
					else if (Input.GetKeyDown(KeyCode.LeftArrow))
					{
						temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

						if (__instance.selectedFloor != null && __instance.selectedFloor.seqID != 0)
						{
							scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
								__instance.selectedFloor,
								temporaryFloors[__instance.selectedFloor.seqID - 1],
								true
							});
						}
						else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
						{
							scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
								(
									__instance.multiSelectedFloors[0].seqID < __instance.multiSelectPoint.seqID) ?
										temporaryFloors[
											__instance.multiSelectedFloors[0].seqID -
												(
													(__instance.multiSelectedFloors[0].seqID != 0) ?
													1 : 0
												)
											] :
								temporaryFloors[__instance.multiSelectedFloors.Last().seqID - 1],
								__instance.multiSelectPoint,
								false
							});
						}
					}
					else if (Input.GetKeyDown(KeyCode.RightArrow))
					{
						temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

						if (__instance.selectedFloor != null && __instance.selectedFloor.seqID != temporaryFloors.Count - 1)
						{
							scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
								__instance.selectedFloor,
								temporaryFloors[__instance.selectedFloor.seqID + 1],
								true
							});
						}
						else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
						{
							scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
								(__instance.multiSelectedFloors.Last().seqID > __instance.multiSelectPoint.seqID) ?
								temporaryFloors[__instance.multiSelectedFloors.Last().seqID + ((__instance.multiSelectedFloors.Last().seqID != temporaryFloors.Count - 1) ? 1 : 0)] :
								temporaryFloors[__instance.multiSelectedFloors[0].seqID + 1], __instance.multiSelectPoint, false
							});
						}
					}
					else if (Input.GetKeyDown(KeyCode.F5))
					{
						char floorType3 = '6';
						for (int l = 0; l < 5; l++)
						{
							scnEditorPrivates.InvokeMethod("CreateFloor", new object[] {
								floorType3, true, false
							});
						}
					}
					else if (Input.GetKeyDown(KeyCode.F7))
					{
						char floorType4 = '8';
						for (int m = 0; m < 7; m++)
						{
							scnEditorPrivates.InvokeMethod("CreateFloor", new object[] {
								floorType4, true, false
							});
						}
					}
				} // end of shift
				else if (Input.GetKeyDown(KeyCode.C))
				{
					if (__instance.selectedFloor != null)
					{
						scnEditorPrivates.InvokeMethod("CopyFloor", new object[] {
							__instance.selectedFloor, true, false
						});
					}
					else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] {
							false
						});
					}
				}
				else if (Input.GetKeyDown(KeyCode.X))
				{
					if (__instance.selectedFloor != null)
					{
						if (__instance.selectedFloor.seqID != 0)
						{
							scnEditorPrivates.InvokeMethod("CutFloor", new object[] {
								__instance.selectedFloor, true
							});
						}
					}
					else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
					{
						scnEditorPrivates.InvokeMethod("MultiCutFloors");
					}
				}
				else if (Input.GetKeyDown(KeyCode.V))
				{
					if (__instance.clipboardCharsEvents.Any())
					{
						if (__instance.selectedFloor != null)
						{
							if (__instance.clipboardCharsEvents.Count() == 1)
							{
								List<LevelEvent> item3 = __instance.clipboardCharsEvents[0].Item2;

								scnEditorPrivates.InvokeMethod("PasteEvents", new object[] {
									__instance.selectedFloor, item3, true, true
								});
							}
							else
							{
								scnEditorPrivates.InvokeMethod("PasteFloors");
							}
						}
						else if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
						{
							if (__instance.clipboardCharsEvents.Count() == 1)
							{
								if (__instance.multiSelectedFloors[0].seqID != 0)
								{
									List<LevelEvent> item4 = __instance.clipboardCharsEvents[0].Item2;

									scnEditorPrivates.InvokeMethod("PasteEvents", new object[] {
										__instance.multiSelectedFloors[0], item4, true, true
									});
								}
							}
							else
							{
								scnEditorPrivates.InvokeMethod("DeleteMultiSelection", new object[] { true });
								scnEditorPrivates.InvokeMethod("PasteFloors");
							}
						}
					}
				}
				else if (Input.GetKeyDown(KeyCode.L))
				{
					if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
					{
						scnEditorPrivates.InvokeMethod("FlipSelection", new object[] {
							true
						});
					}
					else if (__instance.selectedFloor != null)
					{
						scnEditorPrivates.InvokeMethod("FlipFloor", new object[] {
							__instance.selectedFloor, true, true
						});
					}
				}
				else if (Input.GetKeyDown(KeyCode.Comma))
				{
					if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
					{
						scnEditorPrivates.InvokeMethod("RotateSelection", new object[] {
							false
						});
					}
					else if (__instance.selectedFloor != null)
					{
						scnEditorPrivates.InvokeMethod("RotateFloor", new object[] {
							__instance.selectedFloor, false, true
						});
					}
				}
				else if (Input.GetKeyDown(KeyCode.Period))
				{
					if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
					{
						scnEditorPrivates.InvokeMethod("RotateSelection", new object[] {
							true
						});
					}
					else if (__instance.selectedFloor != null)
					{
						scnEditorPrivates.InvokeMethod("RotateFloor", new object[] {
							__instance.selectedFloor, true, true
						});
					}
				}
				else if (Input.GetKeyDown(KeyCode.Slash))
				{
					if (!(bool)scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
					{
						scnEditorPrivates.InvokeMethod("RotateSelection180");
					}
					else if (__instance.selectedFloor != null)
					{
						scnEditorPrivates.InvokeMethod("RotateFloor180", new object[] {
							__instance.selectedFloor, true
						});
					}
				}
				else if (Input.GetKeyDown(KeyCode.H))
				{
					__instance.ToggleShortcutsPanel();
				}
				else if (Input.GetKeyDown(KeyCode.F))
				{
					int num = (__instance.selectedFloor != null) ? __instance.selectedFloor.seqID : -1;
					scnEditorPrivates.SetField("showFloorNums", !(bool)scnEditorPrivates.GetField("showFloorNums"));
					__instance.RemakePath();

					temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

					if (num != -1)
					{
						scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
							temporaryFloors[num], false
						});
					}
				}
				else if (Input.GetKeyDown(KeyCode.U))
				{
					scnEditorPrivates.InvokeMethod("DeselectAnyUIGameObject");
					__instance.ShowPopup(true, scnEditor.PopupType.OpenURL, false);
				}
				else if (Input.GetKeyDown(KeyCode.O))
				{
					scnEditorPrivates.InvokeMethod("DeselectAnyUIGameObject");
					scnEditorPrivates.InvokeMethod("OpenLevel");
				}
				else if (Input.GetKeyDown(KeyCode.S))
				{
					scnEditorPrivates.InvokeMethod("DeselectAnyUIGameObject");
					scnEditorPrivates.InvokeMethod("SaveLevel");
				}
				else if (Input.GetKeyDown(KeyCode.Z))
				{
					__instance.Undo();
				}
				else if (Input.GetKeyDown(KeyCode.Y))
				{
					__instance.Redo();
				}
				else if (Input.GetKeyDown(KeyCode.Delete))
				{
					scnEditorPrivates.InvokeMethod("DeleteSubsequentFloors");
				}
				else if (Input.GetKeyDown(KeyCode.Backspace))
				{
					scnEditorPrivates.InvokeMethod("DeletePrecedingFloors");
				}
				else if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					scnEditorPrivates.InvokeMethod("SelectFirstFloor");
				}
				else if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

					scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
						temporaryFloors[temporaryFloors.Count() - 1], true
					});
				}
				else if (Input.GetKeyDown(KeyCode.Minus))
				{
					float height = Persistence.GetEditorScale() + 50f;
					scnEditorPrivates.InvokeMethod("UpdateCanvasScalerResolution", new object[] { height });
				}
				else if (Input.GetKeyDown(KeyCode.Equals))
				{
					float height2 = Persistence.GetEditorScale() - 50f;
					scnEditorPrivates.InvokeMethod("UpdateCanvasScalerResolution", new object[] { height2 });
				}
				if (!flag2)
				{
					if (Input.GetKeyDown(KeyCode.D))
					{

						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'R', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.W))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'U', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.A))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'L', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.S))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'D', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.X))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'D', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.E))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'E', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.Q))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'Q', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.Z))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'Z', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.C))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { 'C', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.Y))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { key ? 'o' : 'T', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.T))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { key ? 'q' : 'G', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.V))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { key ? 'V' : 'F', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.B))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { key ? 'Y' : 'B', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.J))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { key ? 'p' : 'J', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.H))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { key ? 'W' : 'H', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.N))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { key ? 'x' : 'N', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.M))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { key ? 'A' : 'M', true, false });
					}
					else if (Input.GetKeyDown(KeyCode.Space) && __instance.selectedFloor != null)
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { __instance.lm.GetRotDirection(__instance.lm.GetRotDirection(__instance.selectedFloor.direction, true), true), true, true });
					}
					else if (Input.GetKeyDown(KeyCode.Tab))
					{
						scnEditorPrivates.InvokeMethod("CreateFloor", new object[] { '!', true, true });
					}
				}
				// ----------------------------------------------------------------------------------------- end of keybinding
			}
			if (Input.mouseScrollDelta != Vector2.zero)
			{
				__instance.ShowPropertyHelp(false, null, "", default(Vector2), null, null);
			}
			__instance.floorButtonExtraCanvas.gameObject.SetActive(flag);
			__instance.floorButtonPrimaryCanvas.gameObject.SetActive(!flag);
			__instance.buttonShift.SetEnabled(flag);

			EventIndicator temporaryEvtIndicator = scnEditorPrivates.GetField<EventIndicator>("draggedEvIndicator");

			GameObject[] temporaryObjectsAtMouse = scnEditorPrivates.InvokeMethod<GameObject[]>("ObjectsAtMouse");

			GameObject temporarySmartObjectSelect;

			if (Input.GetMouseButtonUp(0))
			{
				if ((bool) scnEditorPrivates.GetField("dragging"))
				{
					if (temporaryEvtIndicator != null)
					{
						double num2 = (float)temporaryEvtIndicator.floor.entryangle;
						float num3 = -temporaryEvtIndicator.transform.rotation.eulerAngles.z * 0.017453292f;
						double angleMoved = scrMisc.GetAngleMoved(num2, num3, !temporaryEvtIndicator.floor.isCCW);
						temporaryEvtIndicator.evnt.data["angleOffset"] = Mathf.Round((float)angleMoved * 57.29578f);
						__instance.ApplyEventsToFloors();
						__instance.levelEventsPanel.ShowPanelOfEvent(temporaryEvtIndicator.evnt);
						temporaryEvtIndicator.circle.color = Color.white;
						scnEditorPrivates.SetField("draggedEvIndicator", null);
					}
					if ((bool) scnEditorPrivates.GetField("isDraggingTiles"))
					{
						temporaryFloors = scnEditorPrivates.GetProperty<List<scrFloor>>("floors");

						Vector3 zero = Vector3.zero;
						if (!(bool) scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
						{
							new Vector2(0f, 0f);
							new Vector2(0f, 0f);
							bool flag5 = __instance.multiSelectedFloors.First().seqID == 0;
							bool flag6 = __instance.multiSelectedFloors.Last().seqID == temporaryFloors.Count - 1;
							LevelEvent levelEvent = new LevelEvent(__instance.multiSelectedFloors.First().seqID, LevelEventType.PositionTrack, null);
							LevelEvent levelEvent2 = new LevelEvent(__instance.multiSelectedFloors.Last().seqID + 1, LevelEventType.PositionTrack, null);
							__instance.events.RemoveAll((LevelEvent x) => x.floor == __instance.multiSelectedFloors.First().seqID && x.eventType == LevelEventType.PositionTrack);
							__instance.events.RemoveAll((LevelEvent x) => x.floor == __instance.multiSelectedFloors.Last().seqID + 1 && x.eventType == LevelEventType.PositionTrack);
							Vector3 b = Vector3.zero;
							if (!flag5)
							{
								b = temporaryFloors[__instance.multiSelectedFloors.First().seqID - 1].transform.position - temporaryFloors[__instance.multiSelectedFloors.First().seqID - 1].startPos;
							}
							Vector3 a = temporaryFloors[__instance.multiSelectedFloors.First().seqID].transform.position - temporaryFloors[__instance.multiSelectedFloors.First().seqID].startPos;
							Vector3 b2 = Vector3.zero;
							Vector3 a2 = Vector3.zero;
							if (!flag6)
							{
								b2 = temporaryFloors[__instance.multiSelectedFloors.Last().seqID].transform.position - temporaryFloors[__instance.multiSelectedFloors.Last().seqID].startPos;
								a2 = temporaryFloors[__instance.multiSelectedFloors.Last().seqID + 1].transform.position - temporaryFloors[__instance.multiSelectedFloors.Last().seqID + 1].startPos;
							}
							Vector3 vector = a - b;
							levelEvent.data["positionOffset"] = new Vector2(vector.x / __instance.customLevel.GetTileSize(), vector.y / __instance.customLevel.GetTileSize());
							levelEvent.data["editorOnly"] = (flag ? Toggle.Enabled : Toggle.Disabled);
							if (!flag6)
							{
								Vector3 vector2 = a2 - b2;
								levelEvent2.data["positionOffset"] = new Vector2(vector2.x / __instance.customLevel.GetTileSize(), vector2.y / __instance.customLevel.GetTileSize());
								levelEvent2.data["editorOnly"] = (flag ? Toggle.Enabled : Toggle.Disabled);
							}
							__instance.events.Add(levelEvent);
							if (!flag6)
							{
								__instance.events.Add(levelEvent2);
							}
							int seqID4 = __instance.multiSelectedFloors.First().seqID;
							int seqID5 = __instance.multiSelectedFloors.Last().seqID;
							__instance.RemakePath();
							scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
								temporaryFloors[seqID4], temporaryFloors[seqID5], false
							});
						}
						else if (__instance.selectedFloor != null)
						{
							bool flag7 = __instance.selectedFloor.seqID == 0;
							bool flag8 = __instance.selectedFloor.seqID == temporaryFloors.Count - 1;
							LevelEvent levelEvent3 = new LevelEvent(__instance.selectedFloor.seqID, LevelEventType.PositionTrack, null);
							LevelEvent levelEvent4 = new LevelEvent(__instance.selectedFloor.seqID + 1, LevelEventType.PositionTrack, null);
							__instance.events.RemoveAll((LevelEvent x) => x.floor == __instance.selectedFloor.seqID && x.eventType == LevelEventType.PositionTrack);
							__instance.events.RemoveAll((LevelEvent x) => x.floor == __instance.selectedFloor.seqID + 1 && x.eventType == LevelEventType.PositionTrack);
							Vector3 b3 = Vector3.zero;
							if (!flag7)
							{
								b3 = temporaryFloors[__instance.selectedFloor.seqID - 1].transform.position - temporaryFloors[__instance.selectedFloor.seqID - 1].startPos;
							}
							Vector3 a3 = temporaryFloors[__instance.selectedFloor.seqID].transform.position - temporaryFloors[__instance.selectedFloor.seqID].startPos;
							Vector3 b4 = Vector3.zero;
							Vector3 a4 = Vector3.zero;
							if (!flag8)
							{
								b4 = temporaryFloors[__instance.selectedFloor.seqID].transform.position - temporaryFloors[__instance.selectedFloor.seqID].startPos;
								a4 = temporaryFloors[__instance.selectedFloor.seqID + 1].transform.position - temporaryFloors[__instance.selectedFloor.seqID + 1].startPos;
							}
							Vector3 vector3 = a3 - b3;
							levelEvent3.data["positionOffset"] = new Vector2(vector3.x / __instance.customLevel.GetTileSize(), vector3.y / __instance.customLevel.GetTileSize());
							if (!flag8)
							{
								Vector3 vector4 = a4 - b4;
								levelEvent4.data["positionOffset"] = new Vector2(vector4.x / __instance.customLevel.GetTileSize(), vector4.y / __instance.customLevel.GetTileSize());
							}
							__instance.events.Add(levelEvent3);
							if (!flag8)
							{
								__instance.events.Add(levelEvent4);
							}
							int seqID6 = __instance.selectedFloor.seqID;
							__instance.RemakePath();

							scnEditorPrivates.InvokeMethod("SelectFloor", new object[] {
								temporaryFloors[seqID6], true
							});
						}

						scnEditorPrivates.SetField("isDraggingTiles", false);
					}
				}

				if (temporaryObjectsAtMouse != null)
                {
					temporarySmartObjectSelect = scnEditorPrivates.InvokeMethod<GameObject>("SmartObjectSelect", new object[] { true, false });
				}
				else
                {
					temporarySmartObjectSelect = null;
				}
				
				Transform transform = (temporaryObjectsAtMouse != null) ? temporarySmartObjectSelect.transform : null;
				if (!EventSystem.current.IsPointerOverGameObject() && !(bool) scnEditorPrivates.GetField("dragging"))
				{
					if (transform != null)
					{
						scrFloor floor = transform.transform.parent.GetComponent<scrFloor>();
						if (flag && floor != __instance.selectedFloor)
						{
							if (__instance.selectedFloor != null)
							{
								scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] {
									__instance.selectedFloor, floor, true
								});
							}
							else if (!(bool) scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
							{
								scrFloor scrFloor = __instance.multiSelectedFloors[0];
								__instance.multiSelectedFloors.Last();
								if (floor == __instance.multiSelectPoint)
								{
									scnEditorPrivates.InvokeMethod("SelectFloor", new object[] { floor, true });
								}
								else
								{
									scnEditorPrivates.InvokeMethod("MultiSelectFloors", new object[] { floor, __instance.multiSelectPoint, false });
								}
							}
						}
						else
						{
							scnEditorPrivates.InvokeMethod("SelectFloor", new object[] { floor, true });
							__instance.events.Count((LevelEvent x) => x.floor == floor.seqID);
						}
					}
					else if (!EventSystem.current.IsPointerOverGameObject())
					{
						scnEditorPrivates.InvokeMethod("DeselectFloors");
					}
				}
			}
			if ((bool) scnEditorPrivates.GetProperty("paused"))
			{
				GameObject[] array = temporaryObjectsAtMouse;
				if (Input.GetMouseButtonUp(0))
				{
					__instance.ShowPropertyHelp(false, null, "", default(Vector2), null, null);
				}

				Dictionary<scrFloor, Vector3> temporaryfloorPositionsAtDragStart;

				if (Input.GetMouseButtonDown(0))
				{
					scnEditorPrivates.SetField("isDraggingTiles", false);

					if (array != null)
                    {
						temporarySmartObjectSelect = scnEditorPrivates.InvokeMethod<GameObject>("SmartObjectSelect", new object[] { true, true });
					}
					else
                    {
						temporarySmartObjectSelect = null;
					}

					Transform transform2 = (array != null) ? temporarySmartObjectSelect.transform : null;
					if (!EventSystem.current.IsPointerOverGameObject() && flag4 && transform2 != null)
					{
						scrFloor component = transform2.transform.parent.GetComponent<scrFloor>();
						if (!(bool) scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
						{
							if (component.seqID >= __instance.multiSelectedFloors.First().seqID && component.seqID <= __instance.multiSelectedFloors.Last().seqID)
							{
								scnEditorPrivates.SetField("isDraggingTiles", true);
							}
						}
						else if (__instance.selectedFloor != null && component.seqID == __instance.selectedFloor.seqID)
						{
							scnEditorPrivates.SetField("isDraggingTiles", true);
						}
					}
					if ((bool) scnEditorPrivates.GetField("isDraggingTiles"))
					{
						temporaryfloorPositionsAtDragStart = scnEditorPrivates.GetField<Dictionary<scrFloor, Vector3>>("floorPositionsAtDragStart");

						Dictionary<scrFloor, Vector3> floorPositionsAtDragStart = temporaryfloorPositionsAtDragStart;
						floorPositionsAtDragStart.Clear();
						scnEditorPrivates.SetField("floorPositionsAtDragStart", floorPositionsAtDragStart);

						if (!(bool) scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
						{
							using (List<scrFloor>.Enumerator enumerator = __instance.multiSelectedFloors.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									scrFloor scrFloor2 = enumerator.Current;
									floorPositionsAtDragStart[scrFloor2] = scrFloor2.transform.position;
									scnEditorPrivates.SetField("floorPositionsAtDragStart", floorPositionsAtDragStart);
								}
								goto IL_1D6E;
							}
						}
						floorPositionsAtDragStart[__instance.selectedFloor] = __instance.selectedFloor.transform.position;
						scnEditorPrivates.SetField("floorPositionsAtDragStart", floorPositionsAtDragStart);
					}
				IL_1D6E:
					scnEditorPrivates.SetField("mousePosition0", Input.mousePosition);
					scnEditorPrivates.SetField("cameraPositionAtDragStart", __instance.controller.camy.transform.position);

					bool flag9 = false;
					if (array != null)
					{
						foreach (GameObject gameObject in array)
						{
							if (gameObject.transform.parent.GetComponent<EventIndicator>() != null)
							{
								scnEditorPrivates.SetField("draggedEvIndicator", gameObject.transform.parent.GetComponent<EventIndicator>());
								flag9 = true;
								break;
							}
						}
					}
					if (!flag9)
					{
						scnEditorPrivates.SetField("draggedEvIndicator", null);
					}
					if (temporaryEvtIndicator != null)
					{
						scnEditorPrivates.SetField("evIndPosAtDragStart", temporaryEvtIndicator.circle.transform.position);
						__instance.levelEventsPanel.ShowPanelOfEvent(temporaryEvtIndicator.evnt);
						temporaryEvtIndicator.circle.color = new Color(0.9f, 0.9f, 0.9f);
					}
					if (EventSystem.current.IsPointerOverGameObject())
					{
						scnEditorPrivates.SetField("cancelDrag", true);
					}
					if (!(bool) scnEditorPrivates.InvokeMethod("CheckPointerInFilePanel"))
					{
						__instance.ShowFileActionsPanel(false);
					}
					if (!(bool) scnEditorPrivates.InvokeMethod("CheckPointerInShortcutsPanel"))
					{
						__instance.ShowShortcutsPanel(false);
						return false;
					}
				}
				else if (Input.GetMouseButton(0))
				{
					Vector3 temporaryVector = new Vector3();

					temporaryVector = scnEditorPrivates.GetField<Vector3>("mousePosition0");

					Camera temporaryCamera = scnEditorPrivates.GetField<Camera>("camera");

					Vector3 vector5 = (Input.mousePosition - temporaryVector) / Screen.height * temporaryCamera.orthographicSize * 2f;
					Vector3 b5 = new Vector3(vector5.x, vector5.y);
					Vector3 vector6 = Vector3.zero;
					if (!(bool) scnEditorPrivates.GetField("cancelDrag"))
					{
						if (temporaryEvtIndicator == null)
						{
							if (!(bool) scnEditorPrivates.GetField("isDraggingTiles"))
							{
								temporaryVector = scnEditorPrivates.GetField<Vector3>("cameraPositionAtDragStart");

								vector6 = temporaryVector - b5;

								temporaryCamera.transform.position = new Vector3(vector6.x, vector6.y, -10f);

								scnEditorPrivates.SetField("camera", temporaryCamera);
							}
							else
							{
								temporaryfloorPositionsAtDragStart = scnEditorPrivates.GetField<Dictionary<scrFloor, Vector3>>("floorPositionsAtDragStart");

								if (!(bool) scnEditorPrivates.InvokeMethod("MultiSelectionIsEmpty"))
								{
									using (List<scrFloor>.Enumerator enumerator = __instance.multiSelectedFloors.GetEnumerator())
									{
										while (enumerator.MoveNext())
										{
											scrFloor scrFloor3 = enumerator.Current;


											Vector3 vector7 = temporaryfloorPositionsAtDragStart[scrFloor3] + b5;

											scrFloor3.transform.position = new Vector3(vector7.x, vector7.y, scrFloor3.transform.position.z);
										}
										goto IL_217E;
									}
								}
								Vector3 vector8 = temporaryfloorPositionsAtDragStart[__instance.selectedFloor] + b5;
								__instance.selectedFloor.transform.position = new Vector3(vector8.x, vector8.y, __instance.selectedFloor.transform.position.z);
							}
						}
						else
						{
							temporaryVector = scnEditorPrivates.GetField<Vector3>("evIndPosAtDragStart");

							vector6 = temporaryVector + b5;
							Vector3 vector9 = vector6 - temporaryEvtIndicator.gameObject.transform.position;
							float num4 = Vector3.Angle(Vector3.up, vector9);
							if (vector9.x < 0f)
							{
								num4 = 360f - num4;
							}
							num4 *= 0.017453292f;
							scrFloor floor2 = temporaryEvtIndicator.floor;
							double num5 = scrMisc.GetAngleMoved((float)floor2.entryangle, (float)floor2.exitangle, !floor2.isCCW);
							if (Mathf.Abs((float)num5) <= Mathf.Pow(10f, -6f))
							{
								num5 = 6.2831854820251465;
							}
							double num6 = scrMisc.GetAngleMoved((float)floor2.entryangle, num4, !floor2.isCCW) / num5;
							float entryangle = (float)floor2.entryangle;
							double num7 = floor2.entryangle + num5 * (floor2.isCCW ? 1 : -1);
							float num8 = Mathf.Lerp(entryangle, (float)num7, (float)num6);
							num8 = (num8 - 2f * (float)floor2.entryangle) * 57.29578f;
							num8 = Mathf.RoundToInt(num8 / 15f) * 15f;
							temporaryEvtIndicator.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num8);
						}
					}
				IL_217E:
					temporaryVector = scnEditorPrivates.GetField<Vector3>("cameraPositionAtDragStart");

					if (vector6 != temporaryVector)
					{
						scnEditorPrivates.SetField("dragging", true);
						return false;
					}
				}
				else
				{
					scnEditorPrivates.SetField("dragging", false);
					scnEditorPrivates.SetField("cancelDrag", false);
				}
			}
			return false;
		}
    }
}
