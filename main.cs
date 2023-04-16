using Godot;
using Godot.Collections;
using System;
using System.IO;
using System.Text.RegularExpressions;

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
    }

	public void _on_find_pressed()
	{
		string text = input.Text.ToLower();
		if(String.IsNullOrEmpty(text))
		{
            showError("Input is empty");
            return;
		}
		if (Regex.IsMatch(text, @"[^A-Za-z0-9-]+"))
		{
			showError("Input contains spaces or punctuation");
			return;
		}
		int count = 0;
		if(!counts.TryGetValue(text,out count) || count ==0)
		{
            countLabel.Text = text + " was not found!";
            return;
		}
        if(count == 1)
		{
            countLabel.Text = count + " instance of " + text + " was found!";
        }
		else
		{
            countLabel.Text = count + " instances of " + text + " was found!";
        }
        
	}

	public void _on_load_button_pressed()
	{
		openFile.Show();
    }

	public void _on_open_file_file_selected(string _)
	{
        input.Text = "";
        counts.Clear();
        findButton.Disabled = true;
        input.Editable = false;
        try
		{
			string fileDir = @openFile.CurrentPath;
            if (String.IsNullOrEmpty(fileDir))
            {
                throw new Exception("No File Selected");
            }

            progressBar.Value = 0;
			//load this
			
			GD.Print(fileDir);
            using (StreamReader file = new StreamReader(fileDir))
            {
                string delim = " ";//base assume textfile
                string[] dirParts = fileDir.Split('.');

                if (dirParts.Length > 1)
                {
                    string extension = dirParts[^1];
                    if (extension == "csv")
                    {
                        delim = ",";
                    }
                    else if (extension == "tsv")
                    {
                        delim = "\t";
                    }
                    else if (extension != "txt")
                    {
                        throw new Exception("Unsupported File type, txt, csv, and tsv accepted");
                    }
                }
                else
                {
                    throw new Exception("Ambigious File type");
                }

                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    string[] items = ln.Split(delim);//could be a comma seperated file, need to check
					for(int i = 0; i < items.Length; i++)
					{
                        String str = Regex.Replace(items[i], @"[^A-Za-z0-9-]+", "").ToLower();
						if(counts.ContainsKey(str))
						{
							counts[str] = counts[str]+1;
						}
						else
						{
							counts.Add(str, 1);
						}
                    }

                }
                file.Close();
            }

            findButton.Disabled = false;
			input.Editable = true;
			fileLabel.Text = fileDir;

        } 
		catch (Exception ex)
		{
			GD.PrintErr(ex.Message);
			GD.PrintErr(ex.StackTrace);
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
		errorFile.Show();
    }
}
