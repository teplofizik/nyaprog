namespace Programmer.Project
{
    /// <summary>
    /// Шаг скрипта. Конкретные виды: <see cref="Action"/> (команда),
    /// <see cref="SetStep"/> (присвоение переменной), <see cref="IfStep"/> (ветвление).
    /// Условия задаются только структурным шагом <see cref="IfStep"/>.
    /// </summary>
    public abstract class Step
    {
        /// <summary>
        /// Описание шага (для пользователя).
        /// </summary>
        public string Comment;

        /// <summary>
        /// Подпись шага для отображения.
        /// </summary>
        public virtual string Title => Comment ?? GetType().Name;
    }
}
