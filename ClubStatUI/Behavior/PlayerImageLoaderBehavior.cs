// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Wed 05-Jun-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Wed 05-Jun-2024
// ***********************************************************************
// <copyright file="PlayerImageLoaderBehavior.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;

using ClubStatUI.Infrastructure;

namespace ClubStatUI.Behavior
{
    
    public class PlayerImageLoaderBehavior : Behavior<Image>
    {
        public static readonly BindableProperty UserIdProperty =
            BindableProperty.Create(nameof(UserId), typeof(Guid), typeof(PlayerImageLoaderBehavior), Guid.Empty, propertyChanged: OnUserIdChanged);

        public Guid UserId
        {
            get => (Guid)GetValue(UserIdProperty);
            set => SetValue(UserIdProperty, value);
        }

        private static async void OnUserIdChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PlayerImageLoaderBehavior behavior && newValue is Guid userId && behavior.AssociatedObject != null)
            {
                var viewModel = behavior.AssociatedObject.BindingContext as IPlayerImageLoader;
                if (viewModel != null)
                {
                    var bytes = await viewModel.GetPlayerImageAsync(userId);                    
                    behavior.AssociatedObject.Source = bytes.ToImage();
                }
            }
        }

        protected override void OnAttachedTo(Image bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
        }

        protected override void OnDetachingFrom(Image bindable)
        {
            base.OnDetachingFrom(bindable);
            AssociatedObject = null;
        }

        public Image? AssociatedObject { get; private set; }
    }

}

