using Godot;
using Godot.Collections;
using System;

public partial class main : Control
{
	private Dictionary<string, int> counts;

	private FileDialog openFile;
	private AcceptDialog errorFile;
	
	private ProgressBar progressBar;
	
	private LineEdit input;

	private Label inputLabel;
	private Label countLabel;
	private Label fileLabel;

	private Button findButton;

	private RegEx nonLetters = new RegEx();

	public override void _Ready()
	{
		counts = new Dictionary<string, int>();

		openFile = GetNode<FileDialog>("OpenFile");
		errorFile = GetNode<AcceptDialog>("ErrorWindow");

		progressBar = GetNode<ProgressBar>("LoadProgressBar");

		input = GetNode<LineEdit>("LineEdit");
		inputLabel = input.GetNode<Label>("Label");

		countLabel = GetNode<Label>("CountLabel");
        fileLabel = GetNode<Label>("FileLabel");

        findButton = GetNode<Button>("Find");
		nonLetters.Compile("(\\s+)|([\\p{P}\\p{S}])");
    }

	public void _on_find_pressed()
	{

	}

	public void _on_load_button_pressed()
	{
		string text = input.Text;
		if(String.IsNullOrEmpty(text))
		{
            showError("Input is empty");
            return;
		}
		if(nonLetters.Search(text).GetEnd()>0)
		{
			showError("Input contains spaces, numbers, or punctuation");
			return;
		}
    }

	public void _on_open_file_file_selected(string file)
	{
        input.Text = "";
        counts.Clear();
        try
		{
            progressBar.Value = 0;
            //load this

            findButton.Disabled = false;
			input.Editable = true;

        } 
		catch (Exception ex)
		{
			showError(ex.Message);

            counts.Clear();
        }
		finally
		{
			progressBar.Value = 0;
			progressBar.Visible = false;

		}
	}

	private void showError(string message)
	{
        errorFile.DialogText = message;
        errorFile.Visible = true;
    }
}
