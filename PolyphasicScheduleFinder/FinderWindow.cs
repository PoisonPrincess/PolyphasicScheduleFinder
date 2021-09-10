using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PolyphasicScheduleFinder
{
    public partial class FinderWindow : Form
    {
        private bool _consoleWrite = false;

        public FinderWindow(bool consoleWrite)
        {
            _consoleWrite = consoleWrite;
            InitializeComponent();
        }

        private void Done_Click(object sender, EventArgs e)
        {
            bool error = false;
            Results.Rows.Clear();
            List<SleepBlock> userSleepTimes = new List<SleepBlock>();

            if (Age.Value < 10 || Age.Value > 100)
            {
                Age.BackColor = Color.Red;
                error = true;
            }
            else Age.BackColor = Color.White;

            if (!Unsure.Checked)
            {
                if (Mono.Value < 1 || Mono.Value > 14)
                {
                    Mono.BackColor = Color.Red;
                    error = true;
                }
                else Mono.BackColor = Color.White;
            }
            else Mono.BackColor = Color.White;
            
            if (!NoRestrictions.Checked)
            {
                if (!Program.checkLongSleepTime(start1.Text + "-" + End1.Text))
                {
                    start1.BackColor = Color.Red;
                    End1.BackColor = Color.Red;
                    error = true;
                }
                else
                {
                    start1.BackColor = Color.White;
                    End1.BackColor = Color.White;
                    userSleepTimes.Add(new SleepBlock(Program.getDifferenceTime(start1.Text, End1.Text), start1.Text, End1.Text));

                    if (!Program.checkLongSleepTime(start2.Text + "-" + End2.Text) || Program.checkSleepOverlaps(start2.Text, End2.Text, userSleepTimes))
                    {
                        if (start2.Text == "" && End2.Text == "")
                        {
                            start2.BackColor = Color.White; End2.BackColor = Color.White;
                            start3.BackColor = Color.White; End3.BackColor = Color.White;
                            start4.BackColor = Color.White; End4.BackColor = Color.White;
                            start5.BackColor = Color.White; End5.BackColor = Color.White;
                            start6.BackColor = Color.White; End6.BackColor = Color.White;
                        }
                        else
                        {
                            start2.BackColor = Color.Red;
                            End2.BackColor = Color.Red;
                            error = true;
                        }
                    }
                    else
                    {
                        start2.BackColor = Color.White;
                        End2.BackColor = Color.White;

                        userSleepTimes.Add(new SleepBlock(Program.getDifferenceTime(start2.Text, End2.Text), start2.Text, End2.Text));

                        if (!Program.checkLongSleepTime(start3.Text + "-" + End3.Text) || Program.checkSleepOverlaps(start3.Text, End3.Text, userSleepTimes))
                        {
                            if (start3.Text == "" && End3.Text == "")
                            {
                                start3.BackColor = Color.White; End3.BackColor = Color.White;
                                start4.BackColor = Color.White; End4.BackColor = Color.White;
                                start5.BackColor = Color.White; End5.BackColor = Color.White;
                                start6.BackColor = Color.White; End6.BackColor = Color.White;
                            }
                            else
                            {
                                start3.BackColor = Color.Red;
                                End3.BackColor = Color.Red;
                                error = true;
                            }
                        }
                        else
                        {
                            start3.BackColor = Color.White;
                            End3.BackColor = Color.White;

                            userSleepTimes.Add(new SleepBlock(Program.getDifferenceTime(start3.Text, End3.Text), start3.Text, End3.Text));

                            if (!Program.checkLongSleepTime(start4.Text + "-" + End4.Text) || Program.checkSleepOverlaps(start4.Text, End4.Text, userSleepTimes))
                            {
                                if (start4.Text == "" && End4.Text == "")
                                {
                                    start4.BackColor = Color.White; End4.BackColor = Color.White;
                                    start5.BackColor = Color.White; End5.BackColor = Color.White;
                                    start6.BackColor = Color.White; End6.BackColor = Color.White;
                                }
                                else
                                {
                                    start4.BackColor = Color.Red;
                                    End4.BackColor = Color.Red;
                                    error = true;
                                }
                            }
                            else
                            {
                                start4.BackColor = Color.White;
                                End4.BackColor = Color.White;

                                userSleepTimes.Add(new SleepBlock(Program.getDifferenceTime(start4.Text, End4.Text), start4.Text, End4.Text));

                                if (!Program.checkLongSleepTime(start5.Text + "-" + End5.Text) || Program.checkSleepOverlaps(start5.Text, End5.Text, userSleepTimes))
                                {
                                    if (start5.Text == "" && End5.Text == "")
                                    {
                                        start5.BackColor = Color.White; End5.BackColor = Color.White;
                                        start6.BackColor = Color.White; End6.BackColor = Color.White;
                                    }
                                    else
                                    {
                                        start5.BackColor = Color.Red;
                                        End5.BackColor = Color.Red;
                                        error = true;
                                    }
                                }
                                else
                                {
                                    start5.BackColor = Color.White;
                                    End5.BackColor = Color.White;

                                    userSleepTimes.Add(new SleepBlock(Program.getDifferenceTime(start5.Text, End5.Text), start5.Text, End5.Text));

                                    if (!Program.checkLongSleepTime(start6.Text + "-" + End6.Text) || Program.checkSleepOverlaps(start6.Text, End6.Text, userSleepTimes))
                                    {
                                        if (start6.Text == "" && End6.Text == "")
                                        {
                                            start5.BackColor = Color.White; End5.BackColor = Color.White;
                                            start6.BackColor = Color.White; End6.BackColor = Color.White;
                                        }
                                        else
                                        {
                                            start6.BackColor = Color.Red;
                                            End6.BackColor = Color.Red;
                                            error = true;
                                        }
                                    }
                                    else
                                    {
                                        start6.BackColor = Color.White;
                                        End6.BackColor = Color.White;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else 
            {
                start1.BackColor = Color.White; End1.BackColor = Color.White;
                start2.BackColor = Color.White; End2.BackColor = Color.White;
                start3.BackColor = Color.White; End3.BackColor = Color.White;
                start4.BackColor = Color.White; End4.BackColor = Color.White;
                start5.BackColor = Color.White; End5.BackColor = Color.White;
                start6.BackColor = Color.White; End6.BackColor = Color.White;
            }

            if (error) return;
            else
            {
                List<Schedule> schedules = Program.start(decimal.ToInt32(Age.Value), Activity.SelectedIndex, (Unsure.Checked ? 8 : decimal.ToDouble(Mono.Value)), userSleepTimes, NoRestrictions.Checked, Experience.SelectedIndex);

                if (_consoleWrite) Debug.WriteLine("schedules: " + schedules.Count);

                String[] data = new string[3];
                foreach (Schedule s in schedules)
                {
                    data[0] = s.name;
                    data[1] = s.sleeps.Count.ToString();
                    data[2] = s.getTST();
                    Results.Rows.Add(data);
                }
                Results.Update();
            }
        }
    }
}
