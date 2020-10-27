import React, { useEffect, useState, useMemo } from 'react';
import PropTypes from 'prop-types';
import { useDropzone } from 'react-dropzone';
import {
	thumbsContainer,
	thumb,
	thumbInner,
	img,
	baseStyle,
	activeStyle,
	acceptStyle,
	rejectStyle
} from '../layout/PhotoUploadStyles';

const PhotoManagement = ({ currentFiles, setCurrentFiles, newFiles, setNewFiles, deleteRecipePhoto }) => {
	// const allFiles = [ ...newFiles, ...currentFiles ];
	// console.log(...allFiles);

	const removeCurrentFile = (file) => () => {
		const files = [ ...currentFiles ];
		files.splice(files.indexOf(file), 1);
		setCurrentFiles(files);
		deleteRecipePhoto(file.postId, file.postPhotoId);
		console.log(file);
	};

	const removeNewFile = (file) => () => {
		const files = [ ...newFiles ];
		files.splice(files.indexOf(file), 1);
		setNewFiles(files);
	};

	const removeAll = () => {
		setCurrentFiles([]);
	};

	const { getRootProps, getInputProps, isDragActive, isDragAccept, isDragReject, acceptedFiles, open } = useDropzone({
		accept: 'image/*',
		onDrop: (acceptedFiles) => {
			setCurrentFiles(
				acceptedFiles.map((file) =>
					Object.assign(file, {
						preview: URL.createObjectURL(file)
					})
				)
			);
		}
	});

	// Set max number of new files that can be added
	currentFiles.length = Math.min(currentFiles.length, 6);
	newFiles.length = 6 - currentFiles.length;

	const style = useMemo(
		() => ({
			...baseStyle,
			...(isDragActive ? activeStyle : {}),
			...(isDragAccept ? acceptStyle : {}),
			...(isDragReject ? rejectStyle : {})
		}),
		[ isDragActive, isDragReject ]
	);

	const currentPhotoThumbs = currentFiles.map((file) => (
		<div className='thumb-style' style={thumb} key={file.postPhotoId}>
			<div style={thumbInner}>
				<img src={file.url} style={img} />
				<button onClick={removeCurrentFile(file)}>
					<i className='fas fa-trash-alt fa-2x' />
				</button>
			</div>
		</div>
	));

	const newPhotoThumbs = newFiles.map((file) => (
		<div className='thumb-style' style={thumb} key={file.name}>
			<div style={thumbInner}>
				<img src={file.preview} style={img} />
				<button onClick={removeNewFile(file)}>
					<i className='fas fa-trash-alt fa-2x' />
				</button>
			</div>
		</div>
	));

	useEffect(
		() => () => {
			// Make sure to revoke the data uris to avoid memory leaks
			newFiles.forEach((file) => URL.revokeObjectURL(file.preview));
		},
		[ newFiles ]
	);

	return (
		<div className='my-2 text-center'>
			<section className='container'>
				<div {...getRootProps({ style })}>
					<input {...getInputProps()} />
					<p>Drag 'n' drop some files here</p>
					<p>(Maximum of 6 photos)</p>
					<button className='button my-1 btn btn-primary'>Open Files</button>
				</div>
				<aside style={thumbsContainer}>{[ ...newPhotoThumbs, ...currentPhotoThumbs ]}</aside>
				{newFiles.length > 0 && (
					<button className='btn btn-danger' onClick={removeAll}>
						Remove All Newly Added Photos
					</button>
				)}
			</section>
		</div>
	);
};

export default PhotoManagement;
