import React, { Fragment, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import Spinner from '../layout/Spinner';
import CommentItem from './CommentItem';
import { getPost, deletePost, addComment, deleteComment, updateComment } from '../../actions/post';
import { addToFavourites, deleteFavourite, getFavourite } from '../../actions/favourite';
import AwesomeSlider from 'react-awesome-slider';
import AwsSliderStyles from 'react-awesome-slider/dist/styles.css';
import { setAlert } from '../../actions/alert';

const Post = ({
	addToFavourites,
	deleteFavourite,
	getFavourite,
	getPost,
	addComment,
	updateComment,
	deleteComment,
	deletePost,
	history,
	favourite: { favourite, favouritesLoading },
	post: { post, loading },
	auth: { user: authUser },
	// user: { user: { id, username } },
	user: { user: userInStore },
	match,
	location
}) => {
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ requestComments, loadComments ] = useState(false);
	const [ isFavourited, setFavourited ] = useState(false);
	const [ confirmDeletion, setConfirmDeletion ] = useState(false);
	const [ comment, setComment ] = useState('');

	useEffect(
		() => {
			getPost(match.params.id, setLoadingPage);
		},
		[ getPost, match.params.id ]
	);

	useEffect(
		() => {
			if (authUser !== null) {
				getFavourite(authUser.id, match.params.id, setFavourited);
			}
		},
		[ getFavourite, authUser, match.params.id ]
	);

	const onChange = (e) => {
		e.preventDefault();
		setComment(e.target.value);
	};

	const onSubmit = async (e) => {
		e.preventDefault();
		const idOfUser = authUser.id;
		const idOfPost = match.params.id;
		addComment(idOfUser, idOfPost, { comment });
		setComment('');
	};

	if (loadingPage || (authUser !== null && favouritesLoading)) {
		return <Spinner />;
	}

	// console.log(location.state);
	// console.log(id);
	// console.log(username);

	// console.log(authUser);
	// console.log(userInStore.username);
	// console.log(location.state.postsFromCuisine);

	return post === null ? (
		<Spinner />
	) : (
		<Fragment>
			<div className='post-header'>
				{/* {post && id && post.userId === id ? (
					<Link to={`/users/${id}/posts`} className='btn'>
						<i className='fas fa-arrow-circle-left' /> Back To {username.split(' ')[0]}'s Posts
					</Link> */}
				{userInStore && location.state.postsFromProfile ? (
					<Link to={`/users/${userInStore.id}/posts`} className='btn'>
						<i className='fas fa-arrow-circle-left' /> Back To {userInStore.username.split(' ')[0]}'s Posts
					</Link>
				) : userInStore && location.state.favouritesFromProfile ? (
					<Link to={`/users/${userInStore.id}/favourites`} className='btn'>
						<i className='fas fa-arrow-circle-left' /> Back To {userInStore.username.split(' ')[0]}'s
						Favourites
					</Link>
				) : location.state.postsFromCuisine ? (
					// <Link to='/posts' className='btn'>
					// 	<i className='fas fa-arrow-circle-left' /> Back To Cuisine
					// </Link>
					<button onClick={() => history.goBack()}>
						<i className='fas fa-arrow-circle-left' /> Back
					</button>
				) : (
					<Link to='/posts' className='btn'>
						<i className='fas fa-arrow-circle-left' /> Back To Posts
					</Link>
				)}

				{/* Managing state & functionality of the favourite button */}
				<div className='favourites'>
					{!isFavourited && authUser && authUser.id !== post.userId ? (
						<button
							onClick={() => {
								addToFavourites(authUser.id, post.postId, setFavourited);
								// setFavourited(true);
							}}
						>
							<i className='far fa-heart fa-2x text-red' /> <p>Add to favourites</p>
						</button>
					) : authUser === null ? (
						<button
							onClick={() => {
								history.push('/login');
								// console.log(history);
								// addToFavourites(authUser.id, post.postId);
								// toggleFavouriteStatus(false);
							}}
						>
							<i className='fas fa-heart fa-2x text-red' /> <p>Add to favourites</p>
						</button>
					) : (
						isFavourited && (
							<button
								onClick={() => {
									deleteFavourite(authUser.id, post.postId, setFavourited);
								}}
							>
								<i className='fas fa-heart fa-2x text-red' /> <p>Remove from favourites</p>
							</button>
						)
					)}
				</div>

				{/* Handling confirmation of post deletion */}
				<div className='favourites'>
					{authUser &&
					authUser.id === post.userId &&
					!confirmDeletion && (
						<button
							onClick={(e) => {
								setConfirmDeletion(true);
								// deletePost(authUser.id, post.postId, history);
							}}
						>
							Delete post
						</button>
					)}
				</div>

				<div className='favourites'>
					{authUser &&
					authUser.id === post.userId &&
					confirmDeletion && (
						<div>
							Are you sure you want to delete this post?
							<span>
								<button
									onClick={() => {
										deletePost(authUser.id, post.postId, history);
									}}
								>
									Yes
								</button>
								<button onClick={() => setConfirmDeletion(false)}>No</button>
							</span>
						</div>
					)}
				</div>

				{/* Handling edit portion if authenticated */}
				<div className='favourites'>
					{authUser &&
					authUser.id === post.userId && <Link to={`/posts/${post.postId}/edit`}>Edit Post</Link>}
				</div>
			</div>
			<div className='post-grid my-1'>
				<div className='post-top'>
					<h1 className='text-dark'>{post.nameOfDish}</h1>
					{post.postPhoto.length > 0 && (
						<AwesomeSlider cssModule={AwsSliderStyles}>
							{post.postPhoto.map((photo) => <div key={photo.postPhotoId} data-src={photo.url} />)}
						</AwesomeSlider>
					)}
				</div>

				<div className='post-about bg-light p-2 my-3'>
					<h2 className='text-primary'>Description</h2>
					<p>{post.description}</p>
					<div className='line' />

					<div className='info'>
						<div className='p-1'>
							<i className='fas fa-utensils' />
							{' ' + post.cuisine}
						</div>
						<div className='p-1'>
							<i className='far fa-clock' /> Prep - {post.prepTime}
						</div>
						<div className='p-1'>
							<i className='fas fa-clock' /> Cooking - {post.cookingTime}
						</div>
						<div className='p-1'>
							<i className='fas fa-users' /> Serves {post.feeds}
						</div>
					</div>
				</div>
				<div className='post-ingredients bg-white p-2'>
					<h2 className='text-primary'>Ingredients</h2>
					<div>
						{post.ingredients.split(',').map((ingredient) => (
							<div className='p' key={ingredient}>
								<i className='fas fa-angle-right' /> {ingredient}
							</div>
						))}
					</div>
				</div>
				<div className='post-method bg-white p-2'>
					<h2 className='text-primary'>Method</h2>
					<div>{[ ...post.method ]}</div>
				</div>
				<div className='post-footer'>
					{/* <div>
						<h1 className='text-primary'>
							Published by:{' '}
							<Link to='/profile.html'>
								<div className='text-dark'>
									{post.userPhotoUrl ? (
										<img className='icon-b' src={post.userPhotoUrl} />
									) : (
										<i className='far fa-authUser-circle fa-3x ' />
									)}
									<h2>{post.author}</h2>
								</div>
							</Link>
						</h1>
					</div> */}
					{!requestComments ? (
						<button
							className='comments'
							onClick={() =>
								// post.comments.length > 0 ? loadComments(true) : console.log('No comments to load')
								loadComments(true)}
						>
							<i className='fas fa-comments fa-3x text-primary comments'>{'  ' + post.comments.length}</i>
						</button>
					) : (
						<Fragment>
							<div>
								{post.comments.map((comment) => (
									<CommentItem key={comment.commentId} comment={comment} postId={post.postId} />
								))}
							</div>
							<form className='form' onSubmit={(e) => onSubmit(e)}>
								<div className='form-group'>
									<div className='form-group'>
										<textarea
											cols='30'
											rows='5'
											placeholder='Add Comment'
											type='description'
											name='description'
											value={comment}
											required
											onChange={(e) => onChange(e)}
										/>
									</div>
								</div>
								{comment.length === 0 ? (
									<input type='submit' className='btn btn-primary' value='Submit' disabled />
								) : (
									<input type='submit' className='btn btn-primary' value='Submit' />
								)}
							</form>
						</Fragment>
					)}
				</div>
			</div>
		</Fragment>
	);
};

Post.propTypes = {
	getPost: PropTypes.func.isRequired,
	getFavourite: PropTypes.func.isRequired,
	deletePost: PropTypes.func.isRequired,
	addComment: PropTypes.func.isRequired,
	updateComment: PropTypes.func.isRequired,
	deleteComment: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired,
	favourite: PropTypes.object.isRequired,
	user: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth,
	favourite: state.favourite,
	user: state.user
});

export default connect(mapStateToProps, {
	getPost,
	addToFavourites,
	deleteFavourite,
	deletePost,
	getFavourite,
	addComment,
	deleteComment,
	updateComment
})(Post);
