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

            // 1. 判断模式
            bool isReviewMode = args.Contains("-review");

            // 2. 提取所有有效路径（排除以 - 开头的参数和 exe 路径）
            // 注意：args[0] 是 exe 路径，所以从索引 1 开始
            var paths = args
                .Skip(1) // 跳过第一个参数（exe路径）
                .Where(arg => !arg.StartsWith("-") && !string.IsNullOrEmpty(arg))
                .ToList();

            // 3. 创建主窗体（先不传路径，或者传 null）
            this.MainForm = new LabelMinusForm(null, isReviewMode);

            // 调试信息：显示收到的所有参数
            //MessageBox.Show($"程序启动！\n收到参数数量: {args.Length}\n有效文件数量: {paths.Count}", "调试信息");

            // 4. 将提取到的所有路径送入缓冲池
            if (this.MainForm is LabelMinusForm mainForm)
            {
                foreach (var path in paths)
                {
                    mainForm.EnqueuePaths(path, isReviewMode);
                }
            }
        }

        // 当后续实例（双击/右键点击其他文件）启动时，会触发这里
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);

            if (this.MainForm is LabelMinusForm mainForm)
            {
                // 1. 判断模式
                bool isReviewMode = eventArgs.CommandLine.Contains("-review");

                // 2. 提取所有有效路径（排除以 - 开头的参数）
                var paths = eventArgs.CommandLine
                    .Where(arg => !arg.StartsWith("-") && !string.IsNullOrEmpty(arg))
                    .ToList();

                // 3. 唤醒窗体
                if (mainForm.WindowState == FormWindowState.Minimized)
                    mainForm.WindowState = FormWindowState.Normal;
                mainForm.Activate();

                // 4. 将提取到的所有路径送入缓冲池
                // 如果资源管理器分多次调起，EnqueuePaths 内部的 Timer 会等待它们到齐
                foreach (var path in paths)
                {
                    mainForm.EnqueuePaths(path, isReviewMode);
                }
            }
        }
    }
}