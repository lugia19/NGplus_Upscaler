﻿Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Threading
Imports System.Drawing
Imports System.IO.Compression



Module Module1
    Dim Logs As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(Environment.CurrentDirectory & "\NGUpscaler-log.txt", True, System.Text.Encoding.UTF8)
    Dim NGDir As String
    Dim RemasterDir As String
    Dim ParentDir As String = Environment.CurrentDirectory
    Dim scalefraction As String
    Dim ScaleFactor As String
    Dim previnstall As String
    Dim mode As String
    Dim shareddir As String
    Dim choice As String
    Dim ShouldMerge As Boolean
    Dim ListOfMerges As List(Of String) = {"tamamo_st21", "gnome_st33r",
        "008", "009a_05", "015_01", "015_01",
        "015_02", "015_02",
        "015_03", "015_03",
        "015_04", "015_04",
        "017_01", "017_01",
        "017_02", "017_02",
        "017_03", "017_03",
        "017_04", "017_04",
        "017_05", "017_05", "blackwind",
        "elm_poison", "logo", "poseidoness", "poseidoness2", "window1f"}.ToList


    Sub Main()
        Logs.AutoFlush = True
        Logs.WriteLine(vbCrLf & vbCrLf & vbCrLf & vbCrLf & vbCrLf & vbCrLf & DateTime.Now.ToShortDateString & " - " & Now.TimeOfDay.ToString & vbCrLf & vbCrLf & vbCrLf & vbCrLf & vbCrLf & vbCrLf)


        Dim AbsPath As Integer = 0
        Do While AbsPath <> 1 And AbsPath <> 2
            Console.Clear()
            Console.WriteLine("This installer requires that you have both the remaster installed (with the sidestoryupscaler) and have either a build of NG+ or a previous install (saves made in the normal NG+ will work MOSTLY fine)")
            Console.WriteLine("")
            Console.WriteLine("Waifu2x (which is used to upscale the images) requires the Microsoft Visual C++ 2015 Redistributable Update 3.")
            Console.WriteLine("If you don't have it installed (or if you don't know) just run the vc.exe file that's in the mod folder.")
            Console.WriteLine("")
            Console.WriteLine("If you see the error ""libpng warning: iCCP: known incorrect sRGB profile"" just ignore it.")
            Console.WriteLine("There's also a video tutorial linked in the readme. It starts from just the NG+ .zip file and the remaster.")
            Console.WriteLine("Press enter to continue.")
            Console.ReadLine()
            Console.Clear()
            Console.WriteLine("To use this installer you either:")
            Console.WriteLine("")
            Console.WriteLine("1) Need the following folder structure:")
            Console.WriteLine(">Parent directory")
            Console.WriteLine("     >NG+ directory (full install or just the EXTRACTED .zip file of a build)")
            Console.WriteLine("     >Directory where the Remaster is installed")
            Console.WriteLine("     >NG+ upscaler.exe")
            Console.WriteLine("This file must be placed inside the parent directory.")
            Console.WriteLine()
            Console.WriteLine("2) Input the ABSOLUTE path (aka the full path) to the Remaster and NG+'s folders")
            Console.WriteLine("THESE TWO FOLDERS MUST BE SEPARATE AND NOT INSIDE EACHOTHER.")
            Console.WriteLine()
            Console.WriteLine("Please note that moving either of these folders will break the installation and will require running this tool again.")
            Console.WriteLine("It is also possible to update your install of NG+ with a newer build - simply apply the patch and run this tool again.")
            Console.WriteLine()
            Console.WriteLine("Please input your choice")
            Try
                AbsPath = Console.ReadLine()
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Console.WriteLine("Press enter to retry")
                Console.ReadLine()
            End Try
        Loop
        AbsPath = AbsPath - 1
        Console.Clear()
        Do
            If AbsPath Then
                Console.WriteLine("Please input the path to the NG+ install/build:")
                NGDir = Console.ReadLine().Replace("""", "")
            Else
                Console.WriteLine("Please input the folder name of the NG+ install/build:")
                NGDir = Console.ReadLine()
                NGDir = ParentDir & "\" & NGDir
            End If

            If Not (Directory.Exists(NGDir)) Then
                Console.WriteLine("That folder does not exist. Try again.")
            ElseIf Not (File.Exists(NGDir & "\mon_que_ng+.exe")) Then
                Console.WriteLine("The folder does not contain NG+.")
            End If

        Loop While Not Directory.Exists(NGDir) Or Not File.Exists(NGDir & "\mon_que_ng+.exe")

        Console.Clear()
        Do
            If AbsPath Then
                Console.WriteLine("Please input the path to the Remaster install:")
                RemasterDir = Console.ReadLine().Replace("""", "")
            Else
                Console.WriteLine("Please input the folder name of where the Remaster was installed:")
                RemasterDir = Console.ReadLine()
                RemasterDir = ParentDir & "\" & RemasterDir
            End If

            If Not (Directory.Exists(RemasterDir)) Then
                Console.WriteLine("That folder does not exist. Try again.")
            ElseIf Not (File.Exists(RemasterDir & "\Monster Girl Quest Remastered.exe")) Then
                Console.WriteLine("The folder does not contain the remaster.")
            End If

        Loop While Not Directory.Exists(RemasterDir) Or Not File.Exists(RemasterDir & "\Monster Girl Quest Remastered.exe")


        Do
            Console.WriteLine("Use GPU acceleration? (Note: Requires an Nvidia GPU and CUDA, run cuda.exe to install it if you haven't already, it's in the mod folder of the remaster)")
            Console.WriteLine("1) Yes")
            Console.WriteLine("2) No")
            choice = Console.ReadLine()
            If choice = "1" Then
                mode = "gpu"
                Console.Clear()
                Exit Do
            ElseIf choice = "2" Then
                mode = "cpu"
                Console.Clear()
                Exit Do
            End If
            Console.Clear()
        Loop

        Do
            Console.WriteLine("Please enter the number corresponding to your resolution then press Enter to confirm.")
            Console.WriteLine("1) 1080p")
            Console.WriteLine("2) 720p")
            choice = Console.ReadLine()
            If choice = "1" Then
                ScaleFactor = "1.8"
                scalefraction = "*18/10"
                Console.Clear()
                Exit Do
            ElseIf choice = "2" Then
                ScaleFactor = "1.2"
                scalefraction = "*12/10"
                Console.Clear()
                Exit Do
            End If
            Console.Clear()
        Loop



        If Not (File.Exists(RemasterDir & "\mod\SideStoryUpscaler.exe")) Then
            Console.WriteLine("THIS PROGRAM REQUIRES THAT YOU HAVE THE SIDESTORYUPSCALER INSTALLED.")
            Console.WriteLine("PLEASE DOWNLOAD IT FROM THE UPDATES LINK IN THE PASTEBIN AND INSTALL IT IN THE CORRECT LOCATION.")
            Return
        End If

        If Not (Directory.Exists(RemasterDir & "\mod\pngquant")) Then

            Try
                My.Computer.Network.DownloadFile("https://pngquant.org/pngquant-windows.zip", RemasterDir & "\pngquant-windows.zip")
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Console.WriteLine("Please run this with internet access at least the first time - it needs to download the compression software used to save space.")
                File.Delete(RemasterDir & "\pngquant-windows.zip")
                Console.ReadLine()
                Return
            End Try

            Directory.CreateDirectory(RemasterDir & "\mod\pngquant")
            ZipFile.ExtractToDirectory(RemasterDir & "\pngquant-windows.zip", RemasterDir & "\mod\")
            File.Delete(RemasterDir & "\pngquant-windows.zip")
        End If

        Do
            Console.Clear()
            Console.WriteLine("Do you have a previous installation of NG+/the remaster that includes a ""SharedFiles"" folder?")
            Console.WriteLine("(This means you ran a previous version of this tool, if this is the first time just answer no)")
            Console.WriteLine("1) Yes")
            Console.WriteLine("2) No")
            previnstall = Console.ReadLine()
        Loop While Not previnstall = "2" And Not previnstall = "1"

        If previnstall = 1 Then
            'User has picked Yes.

            Do
                Console.Clear()
                Console.WriteLine("Please input the path to the ""SharedFiles"" folder.")
                shareddir = Console.ReadLine().Replace("""", "")
            Loop While Not Directory.Exists(shareddir)

            'We're here and we know we have the correct shareddir folder.
            'Time to copy everything to the NG+/Remaster directories.
            Dim y As New List(Of String)
            y.Add("bg")
            y.Add("chara")
            y.Add("effect")
            y.Add("ency")
            y.Add("se")
            y.Add("system")

            For Each subdir In y
                Try
                    Directory.Delete(RemasterDir & "\" & subdir)        'If it's a junction delete it. This is possible because junctions appear as empty folders.
                    Directory.Delete(NGDir & "\" & subdir)
                Catch ex As Exception

                End Try
                My.Computer.FileSystem.MoveDirectory(shareddir & "\" & subdir, RemasterDir & "\" & subdir, True)
            Next
            Directory.Delete(shareddir)
            'Done moving.
        End If


        'We have established where NG+ and the remaster are installed. Let's start moving the folders.
        'We're leaving the directories in the remaster folder and simply linking NG+ to them.

        'No need to delete these ones beforehand as they don't normally exist
        MakeLink(RemasterDir & "\bg", NGDir & "\bg")
        MakeLink(RemasterDir & "\chara", NGDir & "\chara")



        Try
            If Directory.Exists(NGDir & "\effect") Then
                Directory.Delete(NGDir & "\effect")
            End If

        Catch ex As Exception
            CreateObject("Scripting.FileSystemObject").DeleteFolder(NGDir & "\effect")
        End Try

        MakeLink(RemasterDir & "\effect", NGDir & "\effect")

        Try
            If Directory.Exists(NGDir & "\ency") Then
                Directory.Delete(NGDir & "\ency")
            End If

        Catch ex As Exception
            CreateObject("Scripting.FileSystemObject").DeleteFolder(NGDir & "\ency")
        End Try

        MakeLink(RemasterDir & "\ency", NGDir & "\ency")

        Try
            If Directory.Exists(NGDir & "\se") Then
                Directory.Delete(NGDir & "\se")
            End If

        Catch ex As Exception
            CreateObject("Scripting.FileSystemObject").DeleteFolder(NGDir & "\se")
        End Try
        MakeLink(RemasterDir & "\se", NGDir & "\se")

        Try
            If Directory.Exists(NGDir & "\system") Then
                Directory.Delete(NGDir & "\system")
            End If

        Catch ex As Exception
            CreateObject("Scripting.FileSystemObject").DeleteFolder(NGDir & "\system")
        End Try

        MakeLink(RemasterDir & "\system", NGDir & "\system")


        'Time to copy the miscellaneous files.
        File.Copy(RemasterDir & "\arc.nsa", NGDir & "\arc.nsa", True)
        File.Copy(RemasterDir & "\arc1.nsa", NGDir & "\arc1.nsa", True)
        File.Copy(RemasterDir & "\arc2.nsa", NGDir & "\arc2.nsa", True)
        File.Copy(RemasterDir & "\NSFont.dll", NGDir & "\NSFont.dll", True)
        File.Copy(RemasterDir & "\nslua.dll", NGDir & "\nslua.dll", True)
        File.Copy(RemasterDir & "\nsogg2.dll", NGDir & "\nsogg2.dll", True)
        File.Copy(RemasterDir & "\nspng.dll", NGDir & "\nspng.dll", True)
        File.Copy(RemasterDir & "\SDL.dll", NGDir & "\SDL.dll", True)

        If Not (Directory.Exists(NGDir & "\save")) Then
            Directory.CreateDirectory(NGDir & "\save")
        End If

        'Time to upscale.

        If File.Exists(NGDir & "\2Q\effect\015_01.bmp") Then
            Dim frtemp As FileInfo = New FileInfo(NGDir & "\2Q\effect\015_01.bmp")
            ShouldMerge = MaskCheck(frtemp)
        End If


        Dim fiArr As FileInfo()
        Dim di As New DirectoryInfo(NGDir & "\2Q")
        Dim listoflines As List(Of String)

        Dim count As Integer = 0
        For Each subfolder As DirectoryInfo In di.GetDirectories("*", SearchOption.AllDirectories)
            If subfolder.Name = "bgm" Or subfolder.Name = "se" Then
                Continue For
            End If
            count = 0

            fiArr = subfolder.GetFiles()

            If Not (File.Exists(subfolder.FullName & "\filelist.txt")) Then
                Dim sw2 As StreamWriter = New StreamWriter(subfolder.FullName & "\filelist.txt")
                sw2.Write("")
                sw2.Flush()
                sw2.Close()
            End If

            listoflines = File.ReadAllLines(subfolder.FullName & "\filelist.txt").ToList

            For Each fri In fiArr
                count = count + 1
                Dim friname As String = fri.Name.Substring(0, fri.Name.Length - 4)
                If friname = "minccubus_hd1.jpg" Then
                    Continue For
                End If
                Console.WriteLine("File " & count & " out of " & fiArr.Length)

                If fri.Extension <> ".bmp" And fri.Extension <> ".png" And fri.Extension <> ".jpg" And fri.Extension <> ".jpeg" Then
                    Continue For
                End If

                If listoflines.Contains(friname & ",1") Then   'was in progress, grab the copy
                    Dim tempfriname As String = fri.FullName
                    File.Delete(tempfriname)
                    Do Until File.Exists(tempfriname & "_temp")
                        If tempfriname.Contains("jpeg") Then
                            tempfriname = tempfriname.Replace("jpeg", "wtfbreak")
                        End If
                        If tempfriname.Contains("jpg") Then
                            tempfriname = tempfriname.Replace("jpg", "jpeg")
                        End If
                        If tempfriname.Contains("bmp") Then
                            tempfriname = tempfriname.Replace("bmp", "jpg")
                        End If
                        If tempfriname.Contains("png") Then
                            tempfriname = tempfriname.Replace("png", "bmp")
                        End If
                    Loop
                    If tempfriname.Contains("wtfbreak") Then
                        Console.WriteLine("Something went wrong.")
                        Console.ReadLine()
                    End If
                    File.Copy(tempfriname & "_temp", tempfriname)
                    fri = New FileInfo(tempfriname)


                    Console.WriteLine("File " & friname & " was interrupted, restarting")
                ElseIf listoflines.Contains(friname & ",0") Then   'already upscaled? Check.

                    Console.WriteLine(RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName & """ -format ""%c\n"" info:"))

                    If Not RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName & """ -format ""%c\n"" info:").Contains("Upscaled") Then
                        Console.WriteLine("Index file and metadata mismatch. Reupscaling.")
                        Logs.WriteLine("Index file and metadata mismatch. Reupscaling.")

                        If fri.Extension <> ".png" And File.Exists(fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png") Then
                            Console.WriteLine("Failsafe for duplicates.")
                            Logs.WriteLine("Failsafe for duplicates.")
                            File.Delete(fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png")

                            If subfolder.Name = "effect" Or subfolder.Name = "system" Or subfolder.Name = "chara" Then  'If it's a duplicate, then it's not been edited yet? Hopefully? man, fuck if I know.
                                If ListOfMerges.Contains(friname) And ShouldMerge = True Then
                                    Dim tempfriname = fri.FullName
                                    SplitAndMerge(fri)
                                    fri = New FileInfo(fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png")
                                End If
                            End If

                        End If

                        listoflines.Remove(friname & ",0")      'Remove the old line from the list
                        listoflines.Add(friname & ",1")
                        File.Copy(fri.FullName, fri.FullName & "_temp", True)
                        File.Delete(subfolder.FullName & "\filelist.txt")
                        File.WriteAllLines(subfolder.FullName & "\filelist.txt", listoflines.ToArray)      'Update the file
                        'Console.ReadLine()
                    Else
                        Console.WriteLine("Already upscaled.")
                        Continue For
                    End If

                Else    'It never started upscaling

                    If subfolder.Name = "effect" Or subfolder.Name = "system" Or subfolder.Name = "chara" Then
                        If ListOfMerges.Contains(friname) And ShouldMerge = True Then
                            Dim tempfriname = fri.FullName
                            SplitAndMerge(fri)
                            fri = New FileInfo(fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png")
                        End If
                    End If

                    listoflines.Add(friname & ",1")
                    Dim sw As StreamWriter = File.AppendText(subfolder.FullName & "\filelist.txt")
                    sw.WriteLine(friname & ",1")
                    sw.Flush()
                    sw.Close()
                    File.Copy(fri.FullName, fri.FullName & "_temp", True)
                End If



                'Upscale.
                Console.WriteLine("Upscaling file " & friname)
                Upscale(fri)
                listoflines.Item(listoflines.Count() - 1) = listoflines.Item(listoflines.Count() - 1).Replace(",1", ",0")

                File.Delete(subfolder.FullName & "\filelist.txt")
                File.WriteAllLines(subfolder.FullName & "\filelist.txt", listoflines.ToArray)
                If File.Exists(fri.FullName & "_temp") Then
                    File.Delete(fri.FullName & "_temp")
                End If
            Next
        Next

        Dim savefolderinfo As New DirectoryInfo(NGDir & "\save")
        fiArr = savefolderinfo.GetFiles()

        If Not (File.Exists(savefolderinfo.FullName & "\filelist.txt")) Then
            Dim sw2 As StreamWriter = New StreamWriter(savefolderinfo.FullName & "\filelist.txt")
            sw2.Write("")
            sw2.Flush()
            sw2.Close()
        End If

        listoflines = File.ReadAllLines(savefolderinfo.FullName & "\filelist.txt").ToList   'This upscales the images found in the save folder and converts them to .png
        For Each fri In fiArr
            count = count + 1
            Dim friname As String = fri.Name.Substring(0, fri.Name.Length - 4)
            Console.WriteLine("File " & count & " out of " & fiArr.Length)
            If fri.Extension <> ".bmp" And fri.Extension <> ".png" And fri.Extension <> ".jpg" And fri.Extension <> ".jpeg" Then
                If fri.Extension = ".dat" And Not fri.FullName.Contains("kidoku") And Not fri.FullName.Contains("NScrflog") And Not fri.FullName.Contains("NScrllog") Then
                    Dim bytes As Byte() = File.ReadAllBytes(fri.FullName)
                    Dim IsExt As Boolean = False
                    For i = 0 To bytes.Length - 1
                        'Console.Write(Convert.ToChar(bytes(i)))
                        If Convert.ToChar(bytes(i)) = "\" Then
                            IsExt = True
                            'Console.WriteLine(i)
                            'Console.ReadLine()
                        End If
                        If Convert.ToChar(bytes(i)) = "." And IsExt Then
                            IsExt = False
                            Console.WriteLine(i)
                            Console.WriteLine(Convert.ToChar(bytes(i)) & Convert.ToChar(bytes(i + 1)) & Convert.ToChar(bytes(i + 2)) & Convert.ToChar(bytes(i + 3)))
                            'Console.ReadLine()
                            If Convert.ToChar(bytes(i + 1)) = "j" And Convert.ToChar(bytes(i + 2)) = "p" And Convert.ToChar(bytes(i + 3)) = "g" Then
                                bytes(i + 1) = Encoding.Default.GetBytes("p")(0)
                                bytes(i + 2) = Encoding.Default.GetBytes("n")(0)
                                bytes(i + 3) = Encoding.Default.GetBytes("g")(0)
                            End If

                            If Convert.ToChar(bytes(i + 1)) = "b" And Convert.ToChar(bytes(i + 2)) = "m" And Convert.ToChar(bytes(i + 3)) = "p" Then
                                bytes(i + 1) = Encoding.Default.GetBytes("p")(0)
                                bytes(i + 2) = Encoding.Default.GetBytes("n")(0)
                                bytes(i + 3) = Encoding.Default.GetBytes("g")(0)
                            End If

                        End If

                    Next
                    'Console.ReadLine()

                    File.WriteAllBytes(fri.FullName, bytes)

                End If
                Continue For
            End If
            If listoflines.Contains(friname & ",1") Then   'was in progress, grab the copy
                Dim tempfriname As String = fri.FullName
                File.Delete(tempfriname)
                File.Copy(tempfriname & "_temp", tempfriname)
                fri = New FileInfo(tempfriname)
                Console.WriteLine("File " & friname & " was interrupted, restarting")
            ElseIf listoflines.Contains(friname & ",0") Then   'already upscaled
                Console.WriteLine(RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName & """ -format ""%c\n"" info:"))
                'Console.ReadLine()
                If Not RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName & """ -format ""%c\n"" info:").Contains("Upscaled") Then
                    Logs.WriteLine("File " & friname & " is upscaled according to the list but isn't according to the metadata. Must've been updated.")
                    Logs.WriteLine("Checking the image size.")

                    Console.WriteLine(RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName & """ -format ""%w x %h"" info:"))
                    'Console.ReadLine()

                    If Not RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName & """ -format ""%w x %h"" info:").Contains("120 x 90") Then
                        RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""" & " -set comment ""Upscaled"" """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""")
                        Continue For
                    End If

                    listoflines.Remove(friname & ",0")      'Remove the old line from the list
                    listoflines.Add(friname & ",1")
                    File.Copy(fri.FullName, fri.FullName & "_temp", True)
                    File.Delete(savefolderinfo.FullName & "\filelist.txt")
                    File.WriteAllLines(savefolderinfo.FullName & "\filelist.txt", listoflines.ToArray)      'Update the file

                Else
                    Console.WriteLine("Already upscaled.")
                    If fri.Extension <> ".png" Then
                        fri.Delete()
                    End If
                    Continue For
                End If
            Else    'It never started upscaling. Check if it needs to be upscaled at all or not.

                Console.WriteLine(RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName & """ -format ""%w x %h"" info:"))
                'Console.ReadLine()

                If Not RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName & """ -format ""%w x %h"" info:").Contains("120 x 90") Then
                    listoflines.Add(friname & ",1")
                    Dim sw As StreamWriter = File.AppendText(savefolderinfo.FullName & "\filelist.txt")
                    sw.WriteLine(friname & ",1")
                    sw.Flush()
                    sw.Close()
                    File.Copy(fri.FullName, fri.FullName & "_temp", True)
                Else
                    listoflines.Add(friname & ",0")
                    RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""" & " -set comment ""Upscaled"" """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""")
                    Continue For
                End If
            End If
            'Upscale.
            Console.WriteLine("Upscaling file " & friname)
            Upscale(fri)
            listoflines.Item(listoflines.Count() - 1) = listoflines.Item(listoflines.Count() - 1).Replace(",1", ",0")

            File.Delete(savefolderinfo.FullName & "\filelist.txt")
            File.WriteAllLines(savefolderinfo.FullName & "\filelist.txt", listoflines.ToArray)
            If File.Exists(fri.FullName & "_temp") Then
                File.Delete(fri.FullName & "_temp")
            End If
        Next




        count = 0
        Dim CurrentFile As Integer = 0

        If Not (Directory.Exists(NGDir & "\txtbackups")) Then
            Directory.CreateDirectory(NGDir & "\txtbackups")
        Else    'Means it already started running at some point. Starts over.
            Do While File.Exists(NGDir & "\txtbackups\" & CurrentFile & ".txt")
                File.Delete(NGDir & "\" & CurrentFile & ".txt")
                File.Copy(NGDir & "\txtbackups\" & CurrentFile & ".txt", NGDir & "\" & CurrentFile & ".txt", True)
                CurrentFile = CurrentFile + 1
            Loop
            CurrentFile = 0
        End If

        Console.WriteLine("Done with the upscaling.")
        Console.WriteLine("Starting txt file edits.")


        Do While File.Exists(NGDir & "\" & CurrentFile & ".txt")
            Console.WriteLine("File " & CurrentFile & ".txt")
            'Console.ReadLine()
            If Not (File.Exists(NGDir & "\txtbackups\" & CurrentFile & ".txt")) Then
                File.Move(NGDir & "\" & CurrentFile & ".txt", NGDir & "\txtbackups\" & CurrentFile & ".txt")
            Else
                File.Delete(NGDir & "\" & CurrentFile & ".txt")
            End If

            Dim reader As StreamReader = New StreamReader(NGDir & "\txtbackups\" & CurrentFile & ".txt", System.Text.Encoding.Default)


            Dim a As String
            Dim bodge2 As Boolean = False

            For i = 0 To 30         'Checks like the first 30 lines for the upscaled flag (Just to be safe).
                a = reader.ReadLine
                If a = ";l19-upscaled" Then
                    Console.WriteLine("The file has already been processed.")
                    'Console.ReadLine()
                    reader.Close()
                    Console.WriteLine("File end.")
                    File.Copy(NGDir & "\txtbackups\" & CurrentFile & ".txt", NGDir & "\" & CurrentFile & ".txt", True)
                    CurrentFile = CurrentFile + 1
                    'Console.ReadLine()
                    Continue Do
                End If
            Next

            reader = New StreamReader(NGDir & "\txtbackups\" & CurrentFile & ".txt", System.Text.Encoding.Default)

            Dim writer As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(NGDir & "\" & CurrentFile & ".txt", False, Encoding.Default)
            writer.AutoFlush = True
            Do
                a = reader.ReadLine


                If Not bodge2 Then
                    If a.Contains("S800,600") Then           'Special case for the resolution setting
                        If ScaleFactor = "1.8" Then
                            a = a.Replace("S800", "S1440")
                            a = a.Replace("600L", "1080L")
                        Else
                            a = a.Replace("S800", "S960")
                            a = a.Replace("600L", "720L")
                        End If
                    End If
                    writer.WriteLine(a)

                    writer.WriteLine(";l19-upscaled")

                    bodge2 = True
                    Continue Do
                End If

                Try
                    If a.Contains(".bmp") Then
                        a = a.Replace(".bmp", ".png")
                    End If
                Catch ex As Exception
                    If a Is Nothing Then
                        Console.WriteLine("Assuming it's the last line.")
                        Exit Do
                    End If
                End Try


                If a.Contains(".jpg") Then
                    a = a.Replace(".jpg", ".png")
                End If

                If a.Contains(".jpeg") Then
                    a = a.Replace(".jpeg", ".png")
                End If

                If a.Contains("amsp") And Not a.Contains("%textlastx") Then
                    Console.WriteLine(vbCrLf & a)
                    Dim i As Integer = a.IndexOf(",")
                    a = a.Insert(i + 1, "(")
                    i = a.IndexOf(",", i + 1)
                    a = a.Insert(i, ")" & scalefraction)
                    i = a.IndexOf(",", a.IndexOf(",") + 1)
                    a = a.Insert(i + 1, "(")      'Basically becomes amsp x,y*18/10,18/10*(z)
                    Dim i4 As Integer = i + 1

                    If a.IndexOf(":", i4) <> -1 Then
                        If a.IndexOf(",", i4) <> -1 And a.IndexOf(",", i4) < a.IndexOf(":", i4) Then
                            GoTo commacase2
                        End If
                        i4 = a.IndexOf(":", i4)
                        a = a.Insert(i4, ")" & scalefraction)
                    ElseIf a.IndexOf(";", i4) <> -1 Then
                        If a.IndexOf(",", i4) <> -1 And a.IndexOf(",", i4) < a.IndexOf(";", i4) Then
                            GoTo commacase2
                        End If
                        i4 = a.IndexOf(";", i4)
                        Do While Not (Char.IsNumber(GetChar(a, i4)))
                            Console.WriteLine(GetChar(a, i4))
                            i4 = i4 - 1
                        Loop


                        If GetChar(a, i4 + 1) = "," Then
                            Console.WriteLine(a)
                            Console.WriteLine(GetChar(a, i4 + 3))
                            'Console.ReadLine()
                            If GetChar(a, i4 + 3) = "%" Then
                                i4 = i4 + 3
                                Dim r As Regex = New Regex("[^a-zA-Z0-9()%+]", RegexOptions.IgnoreCase)
                                Dim m As Match
                                m = r.Match(GetChar(a, i4))
                                Do Until m.Success
                                    i4 = i4 + 1
                                    m = r.Match(GetChar(a, i4))
                                Loop
                                a = a.Insert(i4 - 1, ")" & scalefraction)
                                Console.WriteLine(a)
                                'Console.ReadLine()
                            End If

                        Else
                            Dim i5 As Integer = i4
                            Do While Char.IsNumber(GetChar(a, i5))
                                i5 = i5 - 1
                            Loop
                            If GetChar(a, i5) = "+" Then
                                a = a.Insert(i4, ")" & scalefraction)
                            Else
                                a = a.Insert(i4, ")" & scalefraction)
                            End If
                            Console.WriteLine(a)
                            'Console.ReadLine()
                        End If

                        Console.WriteLine(a)
                    ElseIf a.IndexOf(",", i4) <> -1 Then
commacase2:
                        i4 = a.IndexOf(",", i4 + 1)
                        a = a.Insert(i4, ")" & scalefraction)
                    Else
                        a = a & ")" & scalefraction
                    End If

                    Console.WriteLine(a)
                    'Console.ReadLine()
                End If



                If a.Contains("lsp") Then
                    Dim asdjas As String = a
                    a = HandleLSP(a)
                    Console.WriteLine(asdjas)
                    Console.WriteLine(a)
                End If

                If a.Contains("strsp") Then
                    a = HandleSTRSP(a)
                End If

                If a.Contains("getscreenshot") Then
                    Console.WriteLine(a)
                    Dim i As Integer = a.IndexOf("getscreenshot")
                    i = a.IndexOf(",", i)
                    a = a.Insert(i, scalefraction)
                    i = a.IndexOf(",", i) + 1
                    Console.WriteLine(GetChar(a, i))
                    i = i + 1
                    Dim bodge As Boolean = False
                    Do While Char.IsNumber(GetChar(a, i))
                        Console.WriteLine(GetChar(a, i))
                        i = i + 1
                        If i = a.Length Then
                            a = a & scalefraction
                            bodge = True
                        End If
                    Loop
                    If Not (bodge) Then
                        a = a.Insert(i - 1, scalefraction)
                    End If


                    Console.WriteLine(a)
                End If

                If a.Contains("mov $savetime," & """" & ":s/") Or a.Contains("mov $savenum," & """" & ":s/") Then
                    Console.WriteLine(a)
                    Dim i As Integer = a.IndexOf("""")
                    i = a.IndexOf(",", i)
                    If ScaleFactor = "1.8" Then
                        a = a.Insert(i, "+10")
                    Else
                        a = a.Insert(i, "+5")
                    End If
                    i = a.IndexOf(",", i) + 1
                    i = a.IndexOf(",", i)
                    If ScaleFactor = "1.8" Then
                        a = a.Insert(i, "+10")
                    Else
                        a = a.Insert(i, "+5")
                    End If
                    Console.WriteLine(a)
                End If

                If a.Contains("btn ") Then
                    a = Handlebtn(a)
                End If

                If a.Contains("setwindow3") Then
                    a = handlesetwindow(a)
                End If

                If a.Contains("bar ") Then
                    a = Handlebar(a)
                End If

                If a.Contains("mov %monster_x") Then
                    a = Handlemovmonx(a)
                End If

                If a.Contains("mov %monster_y") Then
                    a = handlemovmony(a)
                End If

                If a.Contains("for %") And Not a.Contains("next") Then
                    Dim hasMSP As Boolean = False
                    Dim i As Integer = a.IndexOf("%")
                    Dim variable As String = a.Substring(i, a.IndexOf(" ", i + 1) - i - 2)  'Includes the %
                    i = a.IndexOf("=")
                    Dim min As String = a.Substring(i + 1, a.IndexOf("to", i + 1) - i - 1)
                    Dim stepval As String = 1
                    Dim max As String = 1
                    i = a.IndexOf("to ")
                    If a.Contains("step") Then
                        max = a.Substring(i + 3, a.IndexOf("step", i + 1) - i - 3)
                        i = a.IndexOf("step")

                        If a.Contains(":") Then
                            stepval = a.Substring(i + 4, a.IndexOf(":", i + 1) - i - 4)
                            Console.WriteLine(a)
                            Console.WriteLine(stepval)
                            'Console.ReadLine()
                        Else
                            stepval = a.Substring(i + 4)
                            Console.WriteLine(a)
                            Console.WriteLine(stepval)
                            'Console.ReadLine()
                        End If

                    Else
                        If a.Contains(":") Then
                            max = a.Substring(i + 3, a.IndexOf(":", i + 3) - i - 3)
                        Else
                            max = a.Substring(i + 3)
                        End If
                    End If
                    'We have max, min and stepval.
                    Dim lines As New List(Of String)
                    Dim currentelement As Integer = 0
                    lines.Add(reader.ReadLine)

                    Do While Not lines.Item(currentelement).Contains("next")       'Loop until you find the end of the for loop


                        If lines.Item(currentelement).Contains(".bmp") Then
                            lines.Item(currentelement) = lines.Item(currentelement).Replace(".bmp", ".png")
                        End If

                        If lines.Item(currentelement).Contains(".jpg") Then
                            lines.Item(currentelement) = lines.Item(currentelement).Replace(".jpg", ".png")
                        End If

                        If lines.Item(currentelement).Contains(".jpeg") Then
                            lines.Item(currentelement) = lines.Item(currentelement).Replace(".jpeg", ".png")
                        End If


                        If lines.Item(currentelement).Contains("btn ") Then
                            lines.Item(currentelement) = Handlebtn(lines.Item(currentelement))
                        End If

                        If lines.Item(currentelement).Contains("setwindow3") Then
                            lines.Item(currentelement) = Handlesetwindow(lines.Item(currentelement))
                        End If

                        If lines.Item(currentelement).Contains("bar ") Then
                            lines.Item(currentelement) = handlebar(lines.Item(currentelement))
                        End If

                        If lines.Item(currentelement).Contains("mov %monster_x") Then
                            lines.Item(currentelement) = Handlemovmonx(lines.Item(currentelement))
                        End If

                        If lines.Item(currentelement).Contains("mov %monster_y") Then
                            lines.Item(currentelement) = handlemovmony(lines.Item(currentelement))
                        End If

                        If lines.Item(currentelement).Contains("lsp") Then
                            lines.Item(currentelement) = HandleLSP(lines.Item(currentelement))
                            Console.WriteLine("UHH")
                        End If

                        If lines.Item(currentelement).Contains("strsp") Then
                            lines.Item(currentelement) = HandleSTRSP(lines.Item(currentelement))
                        End If



                        If lines.Item(currentelement).Contains("amsp") Then
                            hasMSP = True
                            i = lines.Item(currentelement).IndexOf(" ")
                            Dim spnum As String = lines.Item(currentelement).Substring(i + 1, lines.Item(currentelement).IndexOf(",", i + 1) - i - 1)
                            i = lines.Item(currentelement).IndexOf(",")
                            Dim tempstring As String = lines.Item(currentelement).Substring(i + 1, lines.Item(currentelement).IndexOf(",", i + 1) - i - 1)

                            Dim valueX As String
                            Dim StepXsign As Char = ""
                            Dim isXstep As Integer = 0

                            If tempstring.Contains("%") Then
                                isXstep = 1
                                If tempstring.IndexOf("-") <> -1 Then
                                    If tempstring.Chars(tempstring.IndexOf("-") + 1) = "%" Then         'If 
                                        StepXsign = "-"
                                        valueX = tempstring.Replace("-" & variable, "")
                                    Else
                                        If tempstring.IndexOf("-", tempstring.IndexOf("-") + 1) <> -1 Then
                                            StepXsign = "-"
                                            valueX = tempstring.Replace("-" & variable, "")
                                        Else
                                            If tempstring.IndexOf("+") <> -1 Then
                                                valueX = tempstring.Replace("+" & variable, "")
                                            Else
                                                valueX = tempstring.Replace(variable, "")
                                            End If
                                        End If
                                    End If
                                Else
                                    valueX = tempstring.Replace(variable, "")
                                    If valueX.Contains("+") Then valueX = valueX.Replace("+", "")
                                End If
                            Else
                                valueX = tempstring
                            End If


                            'Done with getting the default value of X
                            'Repeat everything for Y.
                            Dim ValueY As String
                            Dim StepYsign As Char = ""
                            Dim isYstep As Integer = 0

                            i = lines.Item(currentelement).IndexOf(",", lines.Item(currentelement).IndexOf(",") + 1)
                            i = lines.Item(currentelement).IndexOf(",", i + 1)      'I is the index of the 3rd ,
                            Console.WriteLine(i)
                            Dim i2 As Integer
                            i2 = lines.Item(currentelement).IndexOf(":")
                            If i2 <> -1 And i > i2 Then                     'This is the specific case where the "," is actually located after ":" so it's not part of the command.
                                i = -1
                            End If
                            If i = -1 Then
                                Console.WriteLine(lines.Item(currentelement))
                                'Console.ReadLine()
                                i = lines.Item(currentelement).IndexOf(",", lines.Item(currentelement).IndexOf(",") + 1)     'Back to the 2nd one

                                If lines.Item(currentelement).Contains(":") Then
                                    tempstring = lines.Item(currentelement).Substring(i + 1, lines.Item(currentelement).IndexOf(":", i + 1) - i - 1)
                                Else
                                    tempstring = lines.Item(currentelement).Substring(i + 1)
                                End If

                                Console.WriteLine(tempstring)
                                'Console.ReadLine()

                            Else
                                Console.WriteLine(lines.Item(currentelement))
                                'Console.ReadLine()
                                i = lines.Item(currentelement).IndexOf(",", lines.Item(currentelement).IndexOf(",") + 1)     'Back to the 2nd one

                                tempstring = lines.Item(currentelement).Substring(i + 1, lines.Item(currentelement).IndexOf(",", i + 1) - i - 1)
                                Console.WriteLine(tempstring)
                                'Console.ReadLine()
                            End If



                            If tempstring.Contains("%") Then
                                isYstep = 1
                                If tempstring.IndexOf("-") <> -1 Then
                                    If tempstring.Chars(tempstring.IndexOf("-") + 1) = "%" Then         'If 
                                        StepYsign = "-"
                                        ValueY = tempstring.Replace("-" & variable, "")
                                    Else
                                        If tempstring.IndexOf("-", tempstring.IndexOf("-") + 1) <> -1 Then
                                            StepYsign = "-"
                                            ValueY = tempstring.Replace("-" & variable, "")
                                        Else
                                            If tempstring.IndexOf("+") <> -1 Then
                                                ValueY = tempstring.Replace("+" & variable, "")
                                            Else
                                                ValueY = tempstring.Replace(variable, "")
                                            End If

                                        End If
                                    End If
                                Else
                                    ValueY = tempstring.Replace(variable, "")
                                    If ValueY.Contains("+") Then ValueY = ValueY.Replace("+", "")
                                End If
                            Else
                                ValueY = tempstring
                            End If

                            'We have the default value of Y as well now.
                            Console.WriteLine(a)
                            For Each line In lines
                                Console.WriteLine(line)
                            Next
                            Console.WriteLine(vbCrLf & vbCrLf & vbCrLf)

                            spnum = spnum.Trim()
                            valueX = valueX.Trim()
                            ValueY = ValueY.Trim()
                            stepval = stepval.Trim()

                            writer.WriteLine("amsp " & spnum.Replace(Convert.ToChar(0), "") & "," & valueX.Replace(Convert.ToChar(0), "") & scalefraction & "," & ValueY.Replace(Convert.ToChar(0), "") & scalefraction)
                            Console.WriteLine("amsp " & spnum & "," & valueX & "," & ValueY)



                            lines.Item(currentelement) = "msp " & spnum & "," & StepXsign

                            If isXstep = 1 Then
                                lines.Item(currentelement) = lines.Item(currentelement) & stepval & "," & StepYsign
                            Else
                                lines.Item(currentelement) = lines.Item(currentelement) & "0" & "," & StepYsign
                            End If
                            If isYstep = 1 Then
                                lines.Item(currentelement) = lines.Item(currentelement) & stepval
                            Else
                                lines.Item(currentelement) = lines.Item(currentelement) & "0"
                            End If

                            For Each line In lines
                                Console.WriteLine(line)
                            Next
                            'Console.ReadLine()


                        ElseIf lines.Item(currentelement).Contains("msp") Then             'Just write the modified for instruction
                            hasMSP = True
                            Console.WriteLine(a)
                            Console.WriteLine(lines.Item(currentelement))

                            'Console.ReadLine()
                        ElseIf lines.Item(currentelement).Contains("lsp") Then

                        End If

                        lines.Add(reader.ReadLine)
                        currentelement = currentelement + 1
                    Loop

                    Console.WriteLine("----------------------------------")
                    If hasMSP Then
                        If a.Contains("step") Then
                            writer.WriteLine(a.Insert(a.IndexOf("step") - 1, scalefraction).Replace(Convert.ToChar(0), ""))
                            Console.WriteLine(a.Insert(a.IndexOf("step") - 1, scalefraction))
                        Else
                            writer.WriteLine(a.Replace(Convert.ToChar(0), "") & scalefraction)
                            Console.WriteLine(a & scalefraction)
                        End If
                    Else
                        writer.WriteLine(a.Replace(Convert.ToChar(0), ""))
                    End If


                    For Each line In lines              'Write the contents of the for loop and the "next" statement
                        writer.WriteLine(line.Replace(Convert.ToChar(0), ""))
                        Console.WriteLine(line)
                    Next
                    'Console.ReadLine()
                    Console.WriteLine("*****************")
                End If



                If a.Contains("for %") = False Then
                    writer.WriteLine(a.Replace(Convert.ToChar(0), ""))
                Else
                    If a.Contains("next") Then
                        writer.WriteLine(a.Replace(Convert.ToChar(0), ""))
                    End If
                End If


            Loop Until a Is Nothing
            reader.Close()
            writer.Close()


            Console.WriteLine("File end.")

            CurrentFile = CurrentFile + 1
        Loop


        CreateObject("Scripting.FileSystemObject").DeleteFolder(NGDir & "\txtbackups")



        Logs.Close()
    End Sub

    Sub MakeLink(Link As String, Target As String)
        If Not (Directory.Exists(Target)) Then
            RunCommandH("""C:\Windows\System32\cmd.exe""", " /c mklink /J " & """" & Target & """" & " """ & Link & """")
        End If
    End Sub






    Function RunCommandH(Command As String, Arguments As String) As String
        'Console.WriteLine(Command & Arguments)
        Logs.WriteLine(Command & Arguments)
        'Console.ReadLine()
        Dim oProcess As New Process()
        Dim oStartInfo As New ProcessStartInfo(Command, Arguments)
        oStartInfo.UseShellExecute = False
        oStartInfo.RedirectStandardOutput = True
        oProcess.StartInfo = oStartInfo
        Try
            oProcess.Start()
        Catch ex As Exception
            File.WriteAllText(ParentDir & "crashreport.txt", ex.ToString)
            Console.WriteLine(ParentDir)
            Console.WriteLine(ex.ToString)
            Console.WriteLine("Something went wrong.")
            Console.WriteLine("Please message me on discord (you can find me on the NG+ server, check ecstasywastaken.blogspot.com for the link) and send me the crashreport.txt and NGupscaler-log.txt files.")
            Console.ReadLine()
        End Try



        Dim sOutput As String
        Using oStreamReader As System.IO.StreamReader = oProcess.StandardOutput
            sOutput = oStreamReader.ReadToEnd()
        End Using
        If Not Arguments.Contains("verbose") Or Arguments.Contains("[1x1") Then
            Logs.WriteLine(sOutput)
            'Console.WriteLine(sOutput)
        End If


        oProcess.WaitForExit()
        Return sOutput
        oProcess.WaitForExit()

        oProcess.Dispose()

    End Function

    Sub Upscale(fri As FileInfo)
        RunCommandH("""" & RemasterDir & "\mod\waifu2x\waifu2x-caffe-cui.exe""", "-p " & mode & " -s " & ScaleFactor & " --Model_dir """ & RemasterDir & "\mod\waifu2x\models\anime_style_art_rgb"" -i """ & fri.FullName & """ -o """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""")

        RunCommandH("""" & RemasterDir & "\mod\pngquant\pngquant.exe""", "--skip-if-larger -f --ext _c.png """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""")
        File.Delete(fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png")
        File.Move(fri.FullName.Substring(0, fri.FullName.Length - 4) & "_c.png", fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png")

        RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""" & " -set comment ""Upscaled"" """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""")
        If fri.Extension <> ".png" Then
            fri.Delete()
        End If
    End Sub




    Function MaskCheck(fri As FileInfo) As Boolean

        If fri.Extension <> ".bmp" Then
            Return False
        End If

        If fri.FullName.Contains("black.bmp") Or fri.FullName.Contains("Marco1") Then
            Return True
        End If
        Dim bmp As New Bitmap(fri.FullName)
        Dim FileNameOnly As String = fri.FullName.Substring(0, fri.FullName.Length - 4)
        Dim Corner1, Corner2, Corner3, Corner4 As Boolean

        If Getcolor(fri.FullName, "Red", 0, 0) = 0 And Getcolor(fri.FullName, "Green", 0, 0) = 255 And Getcolor(fri.FullName, "Blue", 0, 0) = 0 Then
            Corner1 = True
        Else
            Corner1 = False
        End If

        If Getcolor(fri.FullName, "Red", 0, bmp.Height - 1) = 0 And Getcolor(fri.FullName, "Green", 0, bmp.Height - 1) = 255 And Getcolor(fri.FullName, "Blue", 0, bmp.Height - 1) = 0 Then
            Corner2 = True
        Else
            Corner2 = False
        End If

        If Getcolor(fri.FullName, "Red", bmp.Width / 2 - 1, 0) = 0 And Getcolor(fri.FullName, "Green", bmp.Width / 2 - 1, 0) = 255 And Getcolor(fri.FullName, "Blue", bmp.Width / 2 - 1, 0) = 0 Then
            Corner3 = True
        Else
            Corner3 = False
        End If

        If Getcolor(fri.FullName, "Red", bmp.Width / 2 - 1, bmp.Height - 1) = 0 And Getcolor(fri.FullName, "Green", bmp.Width / 2 - 1, bmp.Height - 1) = 255 And Getcolor(fri.FullName, "Blue", bmp.Width / 2 - 1, bmp.Height - 1) = 0 Then
            Corner4 = True
        Else
            Corner4 = False
        End If

        bmp.Dispose()
        If Corner1 Or Corner2 Or Corner3 Or Corner4 Then
            Return True
        Else
            Return False
        End If

    End Function

    Function HandleLSP(a As String) As String
        Dim tempa As String = a

        If a.Contains("lsp2") Then
            Logs.WriteLine("lsp2 encountered. Original line: " & a)
        End If
        Console.WriteLine("Original line: " & a)

        Dim bodge As Boolean = False
        If a.IndexOf("lsp") > 0 Then
            Console.WriteLine(a)
            Console.WriteLine(GetChar(a, a.IndexOf("lsp")))
            If GetChar(a, a.IndexOf("lsp")) = ">" Or (a.IndexOf(";") < a.IndexOf("lsp") And a.IndexOf(";") <> -1) Then
                Console.WriteLine(GetChar(a, a.IndexOf("lsp")))
                Console.WriteLine(a.IndexOf(";"))
                Console.WriteLine(a.IndexOf("lsp"))
                Console.WriteLine("it's a comment/whatever the fuck the one with > is.")
                bodge = True
            End If
        End If
        If bodge = False Then
            Dim i As Integer = a.IndexOf("lsp")
            i = a.IndexOf("""", i)
            Console.WriteLine(a.Substring(i + 1, 3))



            If a.Substring(i + 1, 3) = ":s/" Then

                Dim i3 As Integer = a.IndexOf(":s/") + 3
                Dim i2 As Integer = a.IndexOf(",", i3)

                i2 = a.IndexOf(",", i2 + 1)
                Dim currenttextsize As String = a.Substring(i3, i2 - i3)
                Console.WriteLine(currenttextsize)

                Dim size1, size2 As Integer
                Console.WriteLine(currenttextsize.Substring(0, currenttextsize.Length - currenttextsize.IndexOf(",") - 1))
                Console.WriteLine(currenttextsize.Substring(currenttextsize.IndexOf(",") + 1))

                size1 = currenttextsize.Substring(0, currenttextsize.Length - currenttextsize.IndexOf(",") - 1)
                size2 = currenttextsize.Substring(currenttextsize.IndexOf(",") + 1)
                If ScaleFactor = "1.8" Then
                    a = a.Replace(":s/" & size1 & "," & size2, ":s/" & size1 + 10 & "," & size2 + 10)
                Else
                    a = a.Replace(":s/" & size1 & "," & size2, ":s/" & size1 + 4 & "," & size2 + 4)
                End If
                'Text sizes should now be fixed.
                Console.WriteLine(a)

            End If


            i = a.IndexOf("""", i + 1) + 1      'i is now after the " close
            If i = 0 Then
                i = a.IndexOf(",", i) + 1
            End If
            i = a.IndexOf(",", i) + 1
            a = a.Insert(i, "(")
            a = a.Insert(a.IndexOf(",", i), ")" & scalefraction)
            Dim i4 As Integer = a.IndexOf(",", i) + 1   'i4 should be just after the ,
            a = a.Insert(i4, "(")
            Console.WriteLine((GetChar(a, i4)))

            Console.WriteLine(a.IndexOf(":", i4))
            Console.WriteLine(a.IndexOf(";", i4))
            If a.IndexOf(":", i4) <> -1 Then
                If a.IndexOf(",", i4) <> -1 And a.IndexOf(",", i4) < a.IndexOf(":", i4) Then
                    GoTo commacase
                End If
                i4 = a.IndexOf(":", i4)
                a = a.Insert(i4, ")" & scalefraction)
            ElseIf a.IndexOf(";", i4) <> -1 Then
                If a.IndexOf(",", i4) <> -1 And a.IndexOf(",", i4) < a.IndexOf(";", i4) Then
                    GoTo commacase
                End If
                i4 = a.IndexOf(";", i4)
                Do While Not (Char.IsNumber(GetChar(a, i4)))
                    Console.WriteLine(GetChar(a, i4))
                    i4 = i4 - 1
                Loop


                If GetChar(a, i4 + 1) = "," Then
                    Console.WriteLine(a)
                    Console.WriteLine(GetChar(a, i4 + 3))
                    'Console.ReadLine()
                    If GetChar(a, i4 + 3) = "%" Then
                        i4 = i4 + 3
                        Dim r As Regex = New Regex("[^a-zA-Z0-9()%+]", RegexOptions.IgnoreCase)
                        Dim m As Match
                        m = r.Match(GetChar(a, i4))
                        Do Until m.Success
                            i4 = i4 + 1
                            m = r.Match(GetChar(a, i4))
                        Loop
                        a = a.Insert(i4 - 1, ")" & scalefraction)
                        Console.WriteLine(a)

                    End If

                Else
                    Dim i5 As Integer = i4
                    Do While Char.IsNumber(GetChar(a, i5))
                        i5 = i5 - 1
                    Loop
                    If GetChar(a, i5) = "+" Then
                        a = a.Insert(i4, ")" & scalefraction)
                    Else
                        a = a.Insert(i4, ")" & scalefraction)
                    End If
                    Console.WriteLine(a)


                End If

                Console.WriteLine(a)


            ElseIf a.IndexOf(",", i4) <> -1 Then
commacase:
                i4 = a.IndexOf(",", i4 + 1)
                a = a.Insert(i4, ")" & scalefraction)
            Else
                a = a & ")" & scalefraction
            End If

            Console.WriteLine(a)

            If a.Contains("**") Or a.Contains(",*") Or a.Contains(":*") Or a.Contains("lsp2") Then
                Logs.WriteLine("Modified line: " & a)
                'Console.ReadLine()
            End If
            'Positions should now be multiplied correctly.

        End If

        Return a
    End Function

    Function HandleSTRSP(a As String) As String
        Console.WriteLine(a)
        Dim i As Integer = a.IndexOf("strsp")
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i) + 1
        a = a.Insert(i, "(")
        i = a.IndexOf(",", i)
        a = a.Insert(i, ")" & scalefraction)
        i = a.IndexOf(",", i) + 1
        a = a.Insert(i, "(")
        i = a.IndexOf(",", i)
        a = a.Insert(i, ")" & scalefraction)
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        If ScaleFactor = "1.8" Then
            a = a.Insert(i, "+12")
        Else
            a = a.Insert(i, "+4")
        End If
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        If ScaleFactor = "1.8" Then
            a = a.Insert(i, "+12")
        Else
            a = a.Insert(i, "+4")
        End If
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        If ScaleFactor = "1.8" Then
            a = a.Insert(i, "+18")
        Else
            a = a.Insert(i, "+12")
        End If
        Console.WriteLine(a)

        Return a
    End Function

    Function Handlesetwindow(a As String) As String
        Dim i As Integer = a.IndexOf(",")
        a = a.Insert(a.IndexOf(","), scalefraction)
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        a = a.Insert(i, scalefraction)
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        If ScaleFactor = "1.8" Then
            a = a.Insert(i, "+9")
        Else
            a = a.Insert(i, "+4")
        End If
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        If ScaleFactor = "1.8" Then
            a = a.Insert(i, "+9")
        Else
            a = a.Insert(i, "+4")
        End If
        i = a.IndexOf("#")
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        a = a.Insert(i, scalefraction)
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        a = a.Insert(i, scalefraction)
        i = a.IndexOf(",", i) + 1
        i = a.IndexOf(",", i)
        a = a.Insert(i, scalefraction)
        i = a.IndexOf(":", i) - 1
        a = a.Insert(i, scalefraction)

        Console.WriteLine(a)
        Return a
    End Function

    Function handlebar(a As String) As String
        Dim i As Integer = a.IndexOf("bar ") + 4
        Console.WriteLine(a)
        Console.WriteLine(GetChar(a, i + 1))

        If Char.IsNumber(GetChar(a, i + 1)) Then
            i = a.IndexOf(",", i) + 1
            i = a.IndexOf(",", i) + 1
            i = a.IndexOf(",", i)
            a = a.Insert(i, scalefraction)
            i = a.IndexOf(",", i + 1) + 1
            i = a.IndexOf(",", i)
            a = a.Insert(i, scalefraction)
            i = a.IndexOf(",", i + 1) + 1
            i = a.IndexOf(",", i)
            a = a.Insert(i, scalefraction)
            i = a.IndexOf(",", i + 1) + 1
            i = a.IndexOf(",", i)
            a = a.Insert(i, scalefraction)
            Console.WriteLine(a)

        End If
        Return a
    End Function

    Function Handlemovmonx(a As String) As String
        Return a   'Edited to fix an issue where it was multiplying twice due to the lsp command
        Dim i As Integer = a.IndexOf(",")
        Dim indexchar As String
        Console.WriteLine(GetChar(a, i))
        If GetChar(a, i) <> "x" Then        'This handles the cases where it's %monster_xb etc.
            indexchar = GetChar(a, i)
        Else
            indexchar = ""
        End If
        If a.Contains(":") Then         'if there is a command following this one
            i = a.IndexOf(":")
            a = a.Insert(i, ":mul %monster_x" & indexchar & "," & ScaleFactor)
        Else
            a = a & ":mul %monster_x" & indexchar & "," & ScaleFactor
        End If
        Console.WriteLine(a)

        Return a
    End Function

    Function handlemovmony(a As String) As String
        Return a   'Edited to fix an issue where it was multiplying twice due to the lsp command
        Dim i As Integer = a.IndexOf(",")
        Dim indexchar As String
        Console.WriteLine(GetChar(a, i))
        If GetChar(a, i) <> "y" Then        'This handles the cases where it's %monster_xb etc.
            indexchar = GetChar(a, i)
        Else
            indexchar = ""
        End If
        If a.Contains(":") Then         'if there is a command following this one
            i = a.IndexOf(":")
            a = a.Insert(i, ":mul %monster_y" & indexchar & "," & ScaleFactor)
        Else
            a = a & ":mul %monster_y" & indexchar & "," & ScaleFactor
        End If
        Console.WriteLine(a)

        Return a
    End Function

    Function Handlebtn(a As String) As String
        Console.WriteLine(a)
        Dim r As Regex = New Regex("[^a-zA-Z0-9]btn ", RegexOptions.IgnoreCase)
        Dim m As Match
        m = r.Match(a)
        If m.Success Then
            Dim i As Integer = a.IndexOf("btn ")
            i = a.IndexOf(",", i) + 1
            i = a.IndexOf(",", i)
            a = a.Insert(i, scalefraction)
            i = a.IndexOf(",", i) + 1
            i = a.IndexOf(",", i)
            a = a.Insert(i, scalefraction)
            i = a.IndexOf(",", i) + 1
            i = a.IndexOf(",", i)
            a = a.Insert(i, scalefraction)
            i = a.IndexOf(",", i) + 1
            i = a.IndexOf(",", i)
            a = a.Insert(i, scalefraction)
            Console.WriteLine(a)
        End If

        Return a
    End Function


    Sub SplitAndMerge(fri As FileInfo)
        Dim filenameonly As String = fri.FullName.Substring(0, fri.FullName.Length - 4)
        RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert -crop 50%x100% """ & fri.FullName & """ """ & filenameonly & "_%d.png""")
        RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & filenameonly & "_0.png"" ( """ & filenameonly & "_1.png"" -colorspace gray -alpha off -negate ) -compose copy-opacity -composite """ & fri.FullName.Substring(0, fri.FullName.Length - 4) & ".png""")

        File.Delete(filenameonly & "_1.png")
        File.Delete(filenameonly & "_0.png")
        If fri.Extension <> ".png" Then
            File.Delete(fri.FullName)
        End If
    End Sub

    Function Getcolor(File As String, Color As String, X As Double, Y As Double) As Int32
        Select Case Color
            Case "Red"
                Color = "r"
            Case "Green"
                Color = "g"
            Case "Blue"
                Color = "b"
        End Select
        Return Convert.ToInt32(RunCommandH("""" & RemasterDir & "\mod\ImageMagick\magick.exe""", "convert """ & File & """[1x1+" & X & "+" & Y & "] -format ""%[fx: floor(255 * u." & Color & ")]"" info:"))
    End Function

End Module
