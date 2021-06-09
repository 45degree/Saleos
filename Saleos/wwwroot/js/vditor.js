/*
 * Copyright 2021 45degree
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

function initialVditor(initialValue, linkPrefix)
{
    var toolbar;
    toolbar = [
        "emoji",
        "headings",
        "bold",
        "italic",
        "strike",
        "link",
        "|",
        "list",
        "ordered-list",
        "check",
        "|",
        "quote",
        "line",
        "code",
        "inline-code",
        "|",
        "upload",
        "record",
        "table",
        "|",
        "export",
        "fullscreen",
        "preview"
    ];

    //挂载到全局
    window.vditor = new Vditor("vditor", {

        // 获取焦点方法
        focus(md) {
            document.onkeydown = function () {
                // 判断 Ctrl+S
                if (event.ctrlKey == true && event.keyCode == 83) {
                    alert('触发ctrl+s');
                    // 或者 return false;
                    event.preventDefault();
                }
            }
        },
        
        preview: {
            markdown: {
                toc: true,
                mark: true,
                footnotes: true,
                autoSpace: true,
                linkPrefix: linkPrefix
            }
        },

        // 这个是自定义导航栏
        toolbar,
        // 全屏选项
        fullscreen: {
            index: 9999,
        },
        // 展示模式
        mode: "sv",

        //编辑器高度
        // height: window.innerHeight,
        // height: screen.height,
        height: 570,
        //是否展示大纲,手机端自动隐藏就行了
        outline: true,
        //打印调试信息
        debugger: false,
        //是否启动打字机模式(没弄明白这个意思)
        typewriterMode: false,
        //默认提示文本
        placeholder: "欢迎使用本博客",

        // 工具栏配置是否隐藏和固定
        toolbarConfig: {
            // 是否固定工具栏
            pin: false,
        },

        // 是否启用字数统计
        counter: {
            enable: true,
            type: "text",
        },

        tab: "\t",

        // 文件上传配置
        upload: {
            accept: "image/*,.mp3, .wav, .rar",
            token: "test",
            url: "/api/upload/editor",
            linkToImgUrl: "/api/upload/fetch",
            filename(name) {
                return name
                    .replace(/[^(a-zA-Z0-9\u4e00-\u9fa5\.)]/g, "")
                    .replace(/[\?\\/:|<>\*\[\]\(\)\$%\{\}@~]/g, "")
                    .replace("/\\s/g", "");
            },
        },

        after() {
            // articleContent is defined in Editor.cshtml,
            // noinspection JSUnresolvedVariable
            vditor.setValue(initialValue)
        }
    });
}
