"use client";

import { useEditor, EditorContent } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import Placeholder from "@tiptap/extension-placeholder";
import Link from "@tiptap/extension-link";
import Image from "@tiptap/extension-image";
import TaskList from "@tiptap/extension-task-list";
import TaskItem from "@tiptap/extension-task-item";
import { useCallback, useEffect } from "react";
import { Toolbar } from "./Toolbar";

interface DocumentEditorProps {
  content?: Record<string, unknown> | null;
  onUpdate?: (content: Record<string, unknown>) => void;
  editable?: boolean;
  placeholder?: string;
  className?: string;
}

export function DocumentEditor({
  content = null,
  onUpdate,
  editable = true,
  placeholder = "Type '/' for commands...",
  className = "",
}: DocumentEditorProps) {
  const editor = useEditor({
    extensions: [
      StarterKit.configure({
        heading: {
          levels: [1, 2, 3, 4],
        },
        bulletList: {
          keepMarks: true,
          keepAttributes: false,
        },
        orderedList: {
          keepMarks: true,
          keepAttributes: false,
        },
      }),
      Placeholder.configure({
        placeholder,
        includeChildren: true,
      }),
      Link.configure({
        openOnClick: false,
        HTMLAttributes: {
          class: "text-blue-600 underline cursor-pointer",
        },
      }),
      Image.configure({
        HTMLAttributes: {
          class: "max-w-full h-auto rounded-lg",
        },
      }),
      TaskList.configure({
        HTMLAttributes: {
          class: "not-prose",
        },
      }),
      TaskItem.configure({
        HTMLAttributes: {
          class: "flex items-start gap-2 my-1",
        },
        nested: true,
      }),
    ],
    content: content ? JSON.stringify(content) : "",
    editable,
    onUpdate: ({ editor }) => {
      if (onUpdate) {
        const json = editor.getJSON();
        onUpdate(json);
      }
    },
    editorProps: {
      attributes: {
        class:
          "prose prose-sm sm:prose lg:prose-lg xl:prose-xl focus:outline-none max-w-none min-h-[400px] px-4 py-3",
      },
    },
  });

  // Update editor content when content prop changes
  useEffect(() => {
    if (editor && content !== null) {
      const currentContent = editor.getJSON();
      const contentStr = JSON.stringify(content);
      const currentStr = JSON.stringify(currentContent);

      if (contentStr !== currentStr) {
        editor.commands.setContent(content);
      }
    }
  }, [content, editor]);

  const setLink = useCallback(() => {
    if (!editor) return;

    const previousUrl = editor.getAttributes("link").href;
    const url = window.prompt("Enter URL:", previousUrl);

    // cancelled
    if (url === null) {
      return;
    }

    // empty
    if (url === "") {
      editor.chain().focus().extendMarkRange("link").unsetLink().run();
      return;
    }

    // update link
    editor.chain().focus().extendMarkRange("link").setLink({ href: url }).run();
  }, [editor]);

  const addImage = useCallback(() => {
    if (!editor) return;

    const url = window.prompt("Enter image URL:");

    if (url === null || url === "") {
      return;
    }

    editor.chain().focus().setImage({ src: url }).run();
  }, [editor]);

  if (!editor) {
    return (
      <div className="border border-gray-200 rounded-lg p-4 animate-pulse">
        <div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
        <div className="h-4 bg-gray-200 rounded w-1/2 mb-2"></div>
        <div className="h-4 bg-gray-200 rounded w-5/6"></div>
      </div>
    );
  }

  return (
    <div className={`border border-gray-200 rounded-lg overflow-hidden ${className}`}>
      {editable && <Toolbar editor={editor} onSetLink={setLink} onAddImage={addImage} />}
      <EditorContent editor={editor} />
    </div>
  );
}
