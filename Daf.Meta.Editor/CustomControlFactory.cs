// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PropertyTools.Wpf;


namespace Daf.Meta.Editor
{
	/// <summary>
	/// Overrides ErrorControl for default ControlFactory in PropertyGrid. Necessary for dynamically
	/// changing the Visibility-property for controls that display error messages.
	/// The problem this class is designed to solve is expected to be fixed in a future release of PropertyTools.
	/// </summary>
	public class CustomControlFactory : PropertyGridControlFactory
	{
		public override ContentControl CreateErrorControl(PropertyItem pi, object instance, Tab tab, PropertyControlFactoryOptions options)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options), "options was null.");

			if (pi == null)
				throw new ArgumentNullException(nameof(pi), "pi was null.");

			INotifyDataErrorInfo notifyDataErrorInfoInstance = (INotifyDataErrorInfo)instance;

			// Overrides the default error template. Resource can be found in App.xaml.
			if (Application.Current.TryFindResource("PropertyGridValidationTemplate") != null)
			{
				options.ValidationErrorTemplate = (DataTemplate)Application.Current.TryFindResource("PropertyGridValidationTemplate");
			}

			ContentControl errorControl = new()
			{
				ContentTemplate = options.ValidationErrorTemplate,
				Focusable = false
			};

			IValueConverter errorConverter;
			string propertyPath;
			// CustomNotifyDataErrorInfoConverter being used instead of the default in PropertyGrid; NotifyDataErrorInfoConverter
			errorConverter = new CustomNotifyDataErrorInfoConverter(notifyDataErrorInfoInstance!, pi.PropertyName);
			propertyPath = nameof(tab.HasErrors);

			if (notifyDataErrorInfoInstance != null)
			{
				notifyDataErrorInfoInstance.ErrorsChanged += (s, e) =>
				{
					UpdateTabForValidationResults(tab, notifyDataErrorInfoInstance);
					// Refresh bindings for Content and Visibility.
					errorControl.GetBindingExpression(ContentControl.ContentProperty).UpdateTarget();
					errorControl.GetBindingExpression(UIElement.VisibilityProperty).UpdateTarget();
				};
			}

			Binding visibilityBinding = new(propertyPath)
			{
				Converter = errorConverter,
				NotifyOnTargetUpdated = true,
				ValidatesOnNotifyDataErrors = false,
				Source = tab
			};

			Binding contentBinding = new(propertyPath)
			{
				Converter = errorConverter,
				ValidatesOnNotifyDataErrors = false,
				Source = tab
			};

			errorControl.SetBinding(UIElement.VisibilityProperty, visibilityBinding);
			errorControl.SetBinding(ContentControl.ContentProperty, contentBinding);

			return errorControl;
		}
	}
}
