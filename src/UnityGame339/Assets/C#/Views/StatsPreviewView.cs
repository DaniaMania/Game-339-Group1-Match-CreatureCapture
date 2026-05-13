using TMPro;
using UnityEngine;

/// <summary>
/// Shows the player's total stats (current MaxHP and Attack). Can be temporarily put into
/// "preview mode" to display hypothetical values (e.g. what the stats would be after an upgrade).
/// Eventually folds into the persistent left panel; for now it lives inside the upgrade screen.
/// </summary>
public class StatsPreviewView : TypedView<Character>
{
    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _maxHPLabel;
    [SerializeField] private TextMeshProUGUI _attackLabel;

    [Header("Preview Styling")]
    [Tooltip("Color applied to the values while showing a preview, to visually distinguish from current stats.")]
    [SerializeField] private Color _previewColor = new Color(1f, 0.85f, 0.3f);

    private Character _character;
    private Color _baseMaxHPColor = Color.white;
    private Color _baseAttackColor = Color.white;
    private bool _showingPreview;

    protected override void InitializeView(Character[] character)
    {
        _character = character[0];

        if (_maxHPLabel != null) _baseMaxHPColor = _maxHPLabel.color;
        if (_attackLabel != null) _baseAttackColor = _attackLabel.color;

        _character.MaxHP.ChangeEvent += OnStatsChanged;
        _character.Attack.ChangeEvent += OnStatsChanged;

        UpdateCurrentDisplay();
    }

    protected override void DeinitializeView()
    {
        if (_character != null)
        {
            _character.MaxHP.ChangeEvent -= OnStatsChanged;
            _character.Attack.ChangeEvent -= OnStatsChanged;
        }
        _character = null;
        _showingPreview = false;
    }

    /// <summary>
    /// Show hypothetical values instead of the character's current stats. Labels switch to preview color.
    /// </summary>
    public void SetPreview(int previewMaxHP, int previewAttack)
    {
        _showingPreview = true;
        if (_maxHPLabel != null)
        {
            _maxHPLabel.text = previewMaxHP.ToString();
            _maxHPLabel.color = _previewColor;
        }
        if (_attackLabel != null)
        {
            _attackLabel.text = previewAttack.ToString();
            _attackLabel.color = _previewColor;
        }
    }

    /// <summary>
    /// Revert to displaying the character's actual current stats.
    /// </summary>
    public void ClearPreview()
    {
        _showingPreview = false;
        UpdateCurrentDisplay();
    }

    private void OnStatsChanged(int _)
    {
        // Don't overwrite the preview when stats change underneath — preview is intentional.
        if (!_showingPreview) UpdateCurrentDisplay();
    }

    private void UpdateCurrentDisplay()
    {
        if (_character == null) return;
        if (_maxHPLabel != null)
        {
            _maxHPLabel.text = _character.MaxHP.Value.ToString();
            _maxHPLabel.color = _baseMaxHPColor;
        }
        if (_attackLabel != null)
        {
            _attackLabel.text = _character.Attack.Value.ToString();
            _attackLabel.color = _baseAttackColor;
        }
    }
}
