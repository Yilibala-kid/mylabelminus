using Microsoft.VisualBasic.ApplicationServices;

namespace mylabel
{
    internal static class Program
    {

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // 使用控制器运行程序
            var controller = new SingleInstanceController();
            controller.Run(args);
        }
    }
    // 单实例控制器类
    public class SingleInstanceController : WindowsFormsApplicationBase
    {
        public SingleInstanceController()
        {
            // 关键：开启单实例模式
            this.IsSingleInstance = true;
        }

        // 当第一个实例启动时，创建主窗体
        protected override void OnCreateMainForm()
        {
            string[] args = Environment.GetCommandLineArgs();
            // args[0] 是 exe 路径，args[1] 是文件路径
            string? initialPath = args.Length > 1 ? args[1] : null;

            this.MainForm = new LabelMinusForm(initialPath);
        }

        // 当后续实例（双击/右键点击其他文件）启动时，会触发这里
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);

            // 拿到新的文件路径
            // 注意：eventArgs.CommandLine 不包含 exe 路径，索引 0 就是第一个参数
            string? nextPath = eventArgs.CommandLine.Count > 0 ? eventArgs.CommandLine[0] : null;

            if (!string.IsNullOrEmpty(nextPath) && this.MainForm is LabelMinusForm mainForm)
            {
                // 这一步会让窗体在后台收到消息后闪烁或跳到前台
                if (mainForm.WindowState == FormWindowState.Minimized)
                    mainForm.WindowState = FormWindowState.Normal;
                mainForm.Activate();
                // 改用缓冲方法
                mainForm.EnqueuePaths(nextPath);
            }
        }
    }
}