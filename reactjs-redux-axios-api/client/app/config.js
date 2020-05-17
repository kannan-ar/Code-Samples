import React from 'react'

export const config = async () => {
    return await fetch('/config.json').then(res => res.json())
}