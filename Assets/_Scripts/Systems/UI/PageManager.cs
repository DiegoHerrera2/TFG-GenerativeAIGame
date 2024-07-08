using System.Collections;
using System.Collections.Generic;
using _Scripts.Systems.UI;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    // Stack of pages
    private Stack<Page> _pageStack = new Stack<Page>();

    // Push a page onto the stack
    public void PushPage(Page page)
    {
        // If the stack is not empty, close the current page
        if (_pageStack.Count > 0)
        {
            _pageStack.Peek().ClosePage();
        }
        
        // Push the new page onto the stack
        _pageStack.Push(page);
        page.OpenPage();
    }
    
    // Pop a page off the stack
    public void PopPage()
    {
        // If the stack is not empty, close the current page
        if (_pageStack.Count > 0)
        {
            _pageStack.Pop().ClosePage();
        }
        
        // If the stack is not empty, open the new current page
        if (_pageStack.Count > 0)
        {
            _pageStack.Peek().OpenPage();
        }
    }
}
