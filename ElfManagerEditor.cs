using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ElfManager))]
public class ElfManagerEditor : Editor
{
    private void OnSceneGUI()
    {
        ElfManager _elf = (ElfManager)target;

        //For debugging
        Color c = Color.green;
        if (_elf.alertStage == AlertStage.Alerted)
            c = Color.red;

        //Creates the visual FOV arc
        Handles.color = new Color(c.r, c.g, c.b, 0.3f);
        Handles.DrawSolidArc(
            _elf.transform.position, 
            _elf.transform.up, 
            Quaternion.AngleAxis(-_elf.fovAngle / 2f, _elf.transform.up) * _elf.transform.forward, 
            _elf.fovAngle, 
            _elf.fov);
        Handles.color = c;
        _elf.fov = Handles.ScaleValueHandle(_elf.fov, _elf.transform.position + _elf.transform.forward * _elf.fov, _elf.transform.rotation, 3, Handles.SphereHandleCap, 1);
    }
}
