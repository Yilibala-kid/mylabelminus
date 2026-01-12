using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace LabelMinus
{
    namespace LabelMinus
    {
        /// <summary>
        /// 命令接口
        /// </summary>
        public interface ICommand
        {
            void Execute();   // 执行/重做
            void Undo();      // 撤销
        }

        /// <summary>
        /// 撤销管理器：后悔药仓库
        /// </summary>
        public class UndoManager
        {
            private Stack<ICommand> _undoStack = new Stack<ICommand>();
            private Stack<ICommand> _redoStack = new Stack<ICommand>();
            // 核心：记录上次保存时，撤销栈里有多少条记录
            private int _savedStackCount = 0;

            /// <summary>
            /// 判断当前是否有未保存的更改
            /// </summary>
            public bool HasUnsavedChanges => _undoStack.Count != _savedStackCount;

            /// <summary>
            /// 当用户成功执行“保存”操作后，调用此方法同步存盘点
            /// </summary>
            public void MarkAsSaved()
            {
                _savedStackCount = _undoStack.Count;
            }
            // 场景 A：添加/删除。需要立刻执行逻辑并入栈。
            public void Execute(ICommand command)
            {
                command.Execute();
                _undoStack.Push(command);
                _redoStack.Clear(); // 开启新操作时，清空 Redo 栈
            }

            // 场景 B：切换时的状态捕获。逻辑已手动完成，仅将快照入栈。
            public void PushManual(ICommand command)
            {
                _undoStack.Push(command);
                _redoStack.Clear();
            }

            public void Undo()
            {
                if (_undoStack.Count == 0) return;
                ICommand cmd = _undoStack.Pop();
                cmd.Undo();
                _redoStack.Push(cmd);
            }

            public void Redo()
            {
                if (_redoStack.Count == 0) return;
                ICommand cmd = _redoStack.Pop();
                cmd.Execute();
                _undoStack.Push(cmd);
            }

            public void Clear()
            {
                _undoStack.Clear();
                _redoStack.Clear();
                _savedStackCount = 0;
            }
        }

        /// <summary>
        /// 添加/删除标注命令
        /// </summary>
        public class AddDeleteCommand : ICommand
        {
            private readonly BindingList<ImageLabel> _list;
            private readonly ImageLabel _label;
            private readonly ImageInfo _parent;
            private readonly bool _isAdd;

            public AddDeleteCommand(ImageInfo parent, ImageLabel label, bool isAdd)
            {
                _parent = parent;
                _list = parent.Labels;
                _label = label;
                _isAdd = isAdd;
            }

            public void Execute()
            {
                if (_isAdd) _list.Add(_label);
                else _list.Remove(_label);
                _parent.RefreshIndices();
            }

            public void Undo()
            {
                if (_isAdd) _list.Remove(_label);
                else _list.Add(_label);
                _parent.RefreshIndices();
            }
        }



        /// <summary>
        /// 状态快照命令：记录标注切换前后的全状态
        /// </summary>
        public class LabelStateCommand : ICommand
        {
            private readonly ImageLabel _targetLabel; // 原始对象
            private readonly ImageLabel _oldState;    // 旧快照
            private readonly ImageLabel _newState;    // 新快照
            private readonly Action _refreshUI;       // 执行后的回调（如同步UI和重绘）

            public LabelStateCommand(ImageLabel target, ImageLabel oldState, ImageLabel newState, Action refreshUI)
            {
                _targetLabel = target;
                _oldState = oldState;
                _newState = newState;
                _refreshUI = refreshUI;
            }

            public void Execute() => ApplyState(_newState);
            public void Undo() => ApplyState(_oldState);

            private void ApplyState(ImageLabel source)
            {
                if (_targetLabel == null || source == null) return;

                // 自动化：通过反射将 source 的所有属性同步到 _targetLabel
                PropertyInfo[] properties = typeof(ImageLabel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in properties)
                {
                    // Index 属性通常由 ImageInfo 维护，坐标和内容等需要恢复
                    if (prop.CanWrite && prop.Name != "Index")
                    {
                        object value = prop.GetValue(source);
                        prop.SetValue(_targetLabel, value);
                    }
                }
                _refreshUI?.Invoke();
            }
        }


    }
}
