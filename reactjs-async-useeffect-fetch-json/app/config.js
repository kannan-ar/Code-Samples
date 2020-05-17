import React from 'react'

export const Config = async () => {
    return await fetch('/config.json').then(res => res.json())
}
